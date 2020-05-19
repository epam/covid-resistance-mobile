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
using Covi.Client.Services.Platform.Models;
using Covi.Features.UserProfile.Services;
using Covi.Services.Http.Connectivity;
using Covi.Services.Http.ExceptionsHandling;
using Microsoft.Extensions.Logging;

namespace Covi.Services.ApplicationMetadata
{
    public class MetadataService : IMetadataService
    {
        private readonly IMetadataContainer _metadataContainer;
        private readonly IUserProfileContainer _userProfileContainer;
        private readonly IPlatformClient _platformClient;
        private readonly IErrorResponseHandler _serviceErrorHandler;
        private readonly IConnectivityService _connectivityService;
        private readonly ILogger _logger;

        public MetadataService(
            IMetadataContainer metadataContainer,
            IUserProfileContainer userProfileContainer,
            IPlatformClient platformClient,
            IErrorResponseHandler serviceErrorHandler,
            IConnectivityService connectivityService,
            ILoggerFactory loggerFactory)
        {
            _metadataContainer = metadataContainer;
            _userProfileContainer = userProfileContainer;
            _platformClient = platformClient;
            _serviceErrorHandler = serviceErrorHandler;
            _connectivityService = connectivityService;
            _logger = loggerFactory.CreateLogger<UserService>();
        }

        public async Task<Metadata> UpdateMetadataAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await LoadDataInternalAsync(cancellationToken).ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load metadata information.");
                if (_serviceErrorHandler.TryHandle(ex, out var generatedException))
                {
                    generatedException.Rethrow();
                }

                throw;
            }
        }

        private async Task<Metadata> LoadDataInternalAsync(CancellationToken cancellationToken = default)
        {
            _connectivityService.CheckConnection();

            var profileResponse = await _platformClient.Endpoints.GetUserProfileAsync(cancellationToken).ConfigureAwait(false);
            var metadata = profileResponse.Metadata;
            cancellationToken.ThrowIfCancellationRequested();
            await _metadataContainer.SetAsync(metadata).ConfigureAwait(false);
            await _userProfileContainer.SetAsync(profileResponse.UserProfile).ConfigureAwait(false);
            return metadata;
        }
    }
}
