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
using Covi.Features.Account.Services;
using Covi.Services.Http.Connectivity;
using Microsoft.Extensions.Logging;

namespace Covi.Features.MedicalLogin.Services
{
    public class MedicalAuthenticationService : IMedicalAuthenticationService
    {
        private readonly IPlatformClient _client;
        private readonly IAccountContainer _accountContainer;
        private readonly IConnectivityService _connectivityService;
        private readonly IMedicalAuthenticationServiceErrorHandler _serviceErrorHandler;
        private readonly ILogger _logger;

        public MedicalAuthenticationService(
            IPlatformClient client,
            IAccountContainer accountContainer,
            ILoggerFactory loggerFactory,
            IConnectivityService connectivityService,
            IMedicalAuthenticationServiceErrorHandler serviceErrorHandler)
        {
            _client = client;
            _accountContainer = accountContainer;
            _connectivityService = connectivityService;
            _serviceErrorHandler = serviceErrorHandler;
            _logger = loggerFactory.CreateLogger<MedicalAuthenticationService>();
        }

        public async Task<bool> MedicalAuthenticateAsync(string healthSecurityId)
        {
            var isMedicalAuthenticated = false;
            try
            {
                _connectivityService.CheckConnection();

                var profileResponse = await _client.Endpoints
                    .RegisterMedicalAsync(new RegisterDoctorRequest(healthSecurityId)).ConfigureAwait(false);
                if (profileResponse != null)
                {
                    var newAccount = new Account.Models.AccountInformation(profileResponse.UserProfile.Roles);
                    await _accountContainer.SetAsync(newAccount);
                    isMedicalAuthenticated = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to authenticate as medical user.");
                if (_serviceErrorHandler.TryHandle(ex, out var generatedException))
                {
                    generatedException.Rethrow();
                }

                throw;
            }

            return isMedicalAuthenticated;
        }
    }
}
