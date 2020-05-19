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
using System.Threading.Tasks;
using Covi.Services.Http.SessionContainer;
using Microsoft.Extensions.Logging;

namespace Covi.Services.Http.AuthorizationHandling
{
    public class TokenRefreshStrategy : ITokenRefreshStrategy
    {
        private readonly ITokenRefreshInvoker _tokenRefreshInvoker;
        private readonly ISessionContainer _sessionInfoContainer;
        private readonly ILogger _logger;

        public TokenRefreshStrategy(
            ITokenRefreshInvoker tokenRefreshInvoker,
            ILoggerFactory loggerFactory,
            ISessionContainer sessionInfoContainer)
        {
            _tokenRefreshInvoker = tokenRefreshInvoker;
            _logger = loggerFactory.CreateLogger<TokenRefreshStrategy>();
            _sessionInfoContainer = sessionInfoContainer;
        }

        public async Task<bool> TryRefreshSessionAsync(SessionInfo session)
        {
            return await Task.Run(async () =>
            {
                var sessionInfo = session;

                var refreshToken = sessionInfo?.RefreshToken;

                if (refreshToken != null)
                {
                    try
                    {
                        var tokenRefreshResult = await _tokenRefreshInvoker.InvokeRefreshTokenAsync(refreshToken).ConfigureAwait(false);

                        await _sessionInfoContainer.AddOrReplaceAsync(tokenRefreshResult).ConfigureAwait(false);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Cannot refresh the token");
                    }
                }

                return false;
            }).ConfigureAwait(false);
        }
    }
}
