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
using Covi.Features.Account.Services.Authentication;
using Covi.Features.Exceptions;
using Covi.Services.Http.Connectivity;
using Covi.Services.Security.SecretsProvider;
using Microsoft.Extensions.Logging;

namespace Covi.Features.Account.Services.SignUp
{
    public class SignUpService : ISignUpService
    {
        private readonly IPlatformClient _platformClient;
        private readonly IAuthenticationServiceErrorHandler _serviceErrorHandler;
        private readonly IConnectivityService _connectivityService;
        private readonly ISecretsProvider _secretsProvider;
        private readonly IAuthenticationInfoService _authenticationInfoService;
        private readonly ILogger _logger;

        public SignUpService(
            IPlatformClient platformClient,
            ILoggerFactory loggerFactory,
            IAuthenticationServiceErrorHandler serviceErrorHandler,
            IConnectivityService connectivityService,
            ISecretsProvider secretsProvider,
            IAuthenticationInfoService authenticationInfoService)
        {
            _platformClient = platformClient;
            _serviceErrorHandler = serviceErrorHandler;
            _connectivityService = connectivityService;
            _secretsProvider = secretsProvider;
            _authenticationInfoService = authenticationInfoService;
            _logger = loggerFactory.CreateLogger<SignUpService>();
        }

        public async Task CreateProfileAsync(UserSignUpInfo signUpInfo)
        {
            await ExecuteCreateProfileAsync(signUpInfo, true);
        }

        private async Task ExecuteCreateProfileAsync(UserSignUpInfo signUpInfo, bool retryOnTokenException)
        {
            try
            {
                _connectivityService.CheckConnection();

                await _secretsProvider.InitializeAsync().ConfigureAwait(false);
                var identifier = await _secretsProvider.GetDeviceIdentifierAsync().ConfigureAwait(false);

                var request = new RegisterRequest(signUpInfo.Username, signUpInfo.Password, identifier);
                var response = await _platformClient.Endpoints.RegisterUserAsync(request).ConfigureAwait(false);

                await _authenticationInfoService.InitUserInfoAsync(response.Metadata, response.UserProfile, response.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user profile.");
                if (!_serviceErrorHandler.TryHandle(ex, out var generatedException)) throw;

                if (generatedException is CreateProfileTokenValidationException)
                {
                    await ExecuteCreateProfileAsync(signUpInfo, false);
                    return;
                }

                generatedException.Rethrow();
            }
        }
    }
}
