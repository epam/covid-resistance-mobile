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
using Covi.Client.Services;
using Covi.Client.Services.Platform;
using Covi.Client.Services.Platform.Models;
using Covi.Services.Http.Connectivity;
using Microsoft.Extensions.Logging;

namespace Covi.Features.MedicalChangeStatus.Services
{
    public class MedicalChangeStatusService : IMedicalChangeStatusService
    {
        private readonly IPlatformClient _platformClient;
        private readonly IConnectivityService _connectivityService;
        private readonly IMedicalChangeStatusServiceErrorHandler _serviceErrorHandler;
        private readonly ILogger _logger;

        public MedicalChangeStatusService(
            IPlatformClient platformClient,
            ILoggerFactory loggerFactory,
            IConnectivityService connectivityService,
            IMedicalChangeStatusServiceErrorHandler serviceErrorHandler)
        {
            _platformClient = platformClient;
            _connectivityService = connectivityService;
            _serviceErrorHandler = serviceErrorHandler;
            _logger = loggerFactory.CreateLogger<MedicalChangeStatusService>();
        }

        public async Task<MedicalCode> InitStatusChangeAsync(int statusId, DateTime statusChangedOn)
        {
            try
            {
                _connectivityService.CheckConnection();

                var request = new ChangeRequest
                {
                    StatusId = statusId,
                    StatusChangedOn = statusChangedOn.ToUniversalTime()
                };

                return await _platformClient.Endpoints.InitStatusChangeAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to init status change.");
                if (_serviceErrorHandler.TryHandle(ex, out var generatedException))
                {
                    generatedException.Rethrow();
                }

                throw;
            }
        }
    }
}
