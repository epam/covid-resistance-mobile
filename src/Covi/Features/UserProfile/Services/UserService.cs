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
using System.Threading;
using System.Threading.Tasks;
using Covi.Client.Services;
using Covi.Client.Services.Platform;
using Covi.Features.Account.Models;
using Covi.Features.Account.Services;
using Covi.Services.ApplicationMetadata;
using Covi.Services.Http.Connectivity;
using Covi.Services.Http.ExceptionsHandling;
using Microsoft.Extensions.Logging;

namespace Covi.Features.UserProfile.Services
{
    public class UserService : IUserService
    {
        private readonly IPlatformClient _platformClient;
        private readonly IUserProfileContainer _userProfileContainer;
        private readonly IMetadataContainer _metadataContainer;
        private readonly IErrorResponseHandler _serviceErrorHandler;
        private readonly IConnectivityService _connectivityService;
        private readonly IAccountContainer _accountContainer;
        private readonly ILogger _logger;

        public UserService(
            IPlatformClient platformClient,
            IUserProfileContainer userProfileContainer,
            IMetadataContainer metadataContainer,
            ILoggerFactory loggerFactory,
            IErrorResponseHandler serviceErrorHandler,
            IConnectivityService connectivityService,
            IAccountContainer accountContainer)
        {
            _platformClient = platformClient;
            _userProfileContainer = userProfileContainer;
            _metadataContainer = metadataContainer;
            _serviceErrorHandler = serviceErrorHandler;
            _connectivityService = connectivityService;
            _accountContainer = accountContainer;
            _logger = loggerFactory.CreateLogger<UserService>();
        }

        public async Task<Client.Services.Platform.Models.UserState> GetUserStateAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var result = await GetUserStateInternalAsync(cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load user information.");
                if (_serviceErrorHandler.TryHandle(ex, out var generatedException))
                {
                    generatedException.Rethrow();
                }

                throw;
            }
        }

        private async Task<Client.Services.Platform.Models.UserState> GetUserStateInternalAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _connectivityService.CheckConnection();
            var profileResponse = await _platformClient.Endpoints.GetUserProfileAsync(cancellationToken).ConfigureAwait(false);
            var profile = profileResponse.UserProfile;
            cancellationToken.ThrowIfCancellationRequested();
            // metadata should be set first
            await _metadataContainer.SetAsync(profileResponse.Metadata).ConfigureAwait(false);
            await _userProfileContainer.SetAsync(profile).ConfigureAwait(false);
            await _accountContainer.SetAsync(new AccountInformation(profile.Roles)).ConfigureAwait(false);
            return profile;
        }
    }
}
