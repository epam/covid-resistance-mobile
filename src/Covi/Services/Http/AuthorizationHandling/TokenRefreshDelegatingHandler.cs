// =========================================================================
// Copyright 2020 EPAM Systems, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// =========================================================================

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Covi.Services.Http.SessionContainer;
using Nito.AsyncEx;

namespace Covi.Services.Http.AuthorizationHandling
{
    public class TokenRefreshDelegatingHandler : DelegatingHandler
    {
        private const string Bearer = "Bearer";
        private readonly ITokenRefreshStrategy _tokenRefreshStrategy;
        private readonly ISessionContainer _sessionInfoContainer;
        private AsyncReaderWriterLock _tokenRefreshLock = new AsyncReaderWriterLock();

        public TokenRefreshDelegatingHandler(
            ISessionContainer sessionInfoContainer,
            ITokenRefreshStrategy tokenRefreshStrategy)
        {
            _sessionInfoContainer = sessionInfoContainer;
            _tokenRefreshStrategy = tokenRefreshStrategy;
        }

        protected virtual bool ShouldRefreshExpiredToken(HttpResponseMessage response)
        {
            //Check for Forbidden code required for the case, when the user becomes a Medical user and needs the token refreshed.
            return response.StatusCode == HttpStatusCode.Unauthorized ||
                   response.StatusCode == HttpStatusCode.Forbidden;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authToken = await GetAuthTokenAsync().ConfigureAwait(false);

            HttpResponseMessage response = null;

            string requestContent = string.Empty;

            using (await _tokenRefreshLock.ReaderLockAsync())
            {
                // Content is saved for the http message request copy below
                if (request.Content != null)
                {
                    requestContent = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                }

                response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }

            if (ShouldRefreshExpiredToken(response))
            {
                var isTokenRefreshSucceed = true;

                using (await _tokenRefreshLock.WriterLockAsync())
                {
                    // Check whether token hasn't been changed by other request handler
                    // and skip token refresh if changed
                    var session = await GetSessionInfoAsync().ConfigureAwait(false);
                    var currentToken = GetAuthTokenFromSession(session);

                    if (string.Equals(currentToken, authToken, StringComparison.OrdinalIgnoreCase))
                    {
                        isTokenRefreshSucceed = await _tokenRefreshStrategy.TryRefreshSessionAsync(session).ConfigureAwait(false);
                    }
                }

                if (isTokenRefreshSucceed)
                {
                    // Fetch new session
                    authToken = await GetAuthTokenAsync().ConfigureAwait(false);

                    // Request is recreated due to HttpMessageRequest.Content is disposed
                    // after previous call to the backend with Unauthorized response code
                    var recreatedRequest = request.GetCopyWithoutContent();

                    if (!string.IsNullOrEmpty(requestContent))
                    {
                        recreatedRequest.SetContent(requestContent);
                    }

                    ApplyAuthenticationToken(recreatedRequest, authToken);

                    response = await base.SendAsync(recreatedRequest, cancellationToken).ConfigureAwait(false);

                    return response;
                }
            }

            return response;
        }

        private static void ApplyAuthenticationToken(HttpRequestMessage request, string authToken)
        {
            if (!string.IsNullOrEmpty(authToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(Bearer, authToken);
            }
        }

        private async Task<SessionInfo> GetSessionInfoAsync()
        {
            var sessionInfo = await _sessionInfoContainer.GetSessionInfoAsync().ConfigureAwait(false);
            return sessionInfo;
        }

        private string GetAuthTokenFromSession(SessionInfo sessionInfo)
        {
            string authToken = string.Empty;
            if (sessionInfo != null)
            {
                authToken = sessionInfo.AccessToken;
            }

            return authToken;
        }

        private async Task<string> GetAuthTokenAsync()
        {
            var sessionInfo = await _sessionInfoContainer.GetSessionInfoAsync().ConfigureAwait(false);
            return sessionInfo?.AccessToken;
        }
    }
}
