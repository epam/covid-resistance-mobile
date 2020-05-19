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
using Covi.Services.Http.Connectivity;
using Covi.Services.Http.ExceptionsHandling;
using Microsoft.Extensions.Logging;

namespace Covi.Features.Recommendations.Services
{
    public class RecommendationsService : IRecommendationsService
    {
        private readonly IPlatformClient _platformClient;
        private readonly IRecommendationsContainer _recommendationsContainer;
        private readonly IErrorResponseHandler _serviceErrorHandler;
        private readonly IConnectivityService _connectivityService;

        private ILogger<RecommendationsService> _logger;

        public RecommendationsService(
            IPlatformClient platformClient,
            IRecommendationsContainer recommendationsContainer,
            IErrorResponseHandler serviceErrorHandler,
            IConnectivityService connectivityService,
            ILoggerFactory loggerFactory)
        {
            _platformClient = platformClient;
            _recommendationsContainer = recommendationsContainer;
            _serviceErrorHandler = serviceErrorHandler;
            _connectivityService = connectivityService;
            _logger = loggerFactory.CreateLogger<RecommendationsService>();
        }

        public async Task<RecommendationsList> GetRecommendationsAsync(int statusId, CancellationToken cancellationToken = default)
        {
            try
            {
                _connectivityService.CheckConnection();
                cancellationToken.ThrowIfCancellationRequested();

                var recommendationsList = await _platformClient.Endpoints.GetRecommendationsAsync(statusId, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                await _recommendationsContainer.SetAsync(recommendationsList).ConfigureAwait(false);
                return recommendationsList;
            }
            catch (Exception ex)
            {
                if (_serviceErrorHandler.TryHandle(ex, out var generatedException, cancellationToken))
                {
                    if (!(ex is OperationCanceledException))
                    {
                        _logger.LogError(ex, "Failed to load user recommendations.");
                    }
                    generatedException.Rethrow();
                }

                throw;
            }
        }
    }
}
