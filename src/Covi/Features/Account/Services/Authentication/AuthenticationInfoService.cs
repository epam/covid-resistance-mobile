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
using Covi.Client.Services.Platform.Models;
using Covi.Features.UserProfile.Services;
using Covi.Services.ApplicationMetadata;
using Covi.Services.Http.SessionContainer;
using Covi.Services.Security.SecretsProvider;
using Microsoft.Extensions.Logging;

namespace Covi.Features.Account.Services.Authentication
{
    public class AuthenticationInfoService : IAuthenticationInfoService
    {
        private readonly IUserProfileContainer _userProfileContainer;
        private readonly IAccountContainer _accountContainer;
        private readonly ISessionContainer _sessionContainer;
        private readonly IMetadataContainer _metadataContainer;
        private readonly ILogger _logger;

        public AuthenticationInfoService(
            IUserProfileContainer userProfileContainer,
            IAccountContainer accountContainer,
            ISessionContainer sessionContainer,
            IMetadataContainer metadataContainer,
            ILoggerFactory loggerFactory)
        {
            _userProfileContainer = userProfileContainer;
            _accountContainer = accountContainer;
            _sessionContainer = sessionContainer;
            _metadataContainer = metadataContainer;
            _logger = loggerFactory.CreateLogger<AuthenticationInfoService>();
        }

        public async Task InitUserInfoAsync(Metadata metadata, UserState profile, Token token)
        {
            // metadata should be set first
            await _metadataContainer.SetAsync(metadata).ConfigureAwait(false);
            await _userProfileContainer.SetAsync(profile).ConfigureAwait(false);

            var accountInformation = profile == null ? null : new Models.AccountInformation(profile.Roles);
            await _accountContainer.SetAsync(accountInformation).ConfigureAwait(false);

            await _sessionContainer.AddOrReplaceAsync(SessionInfo.CreateFromToken(token)).ConfigureAwait(false);
        }

        public bool IsAuthenticated()
        {
            var result = AsyncHelpers.RunSync(IsAuthenticatedAsync);
            return result;
        }

        private async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var isInitialized = SecretsProvider.Instance.GetIsInitialized();
                if (!isInitialized)
                {
                    return false;
                }

                var result = await _userProfileContainer.GetAsync().ConfigureAwait(false);
                return result != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check user authenticated status.");
                return false;
            }
        }
    }
}
