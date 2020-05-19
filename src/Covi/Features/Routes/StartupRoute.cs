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

using System.Threading.Tasks;
using Covi.Features.Account.Services.Authentication;
using Covi.Features.CreateProfile.Routes;
using Covi.Features.Main.Routes;
using Covi.Features.OnBoarding.Routes;
using Prism.Navigation;

namespace Covi.Features.Routes
{
    public class StartupRoute : IStartupRoute
    {
        private readonly IMainRoute _mainRoute;
        private readonly IOnBoardingRoute _onboardingRoute;
        private readonly ICreateProfileRoute _createProfileRoute;
        private readonly IAuthenticationInfoService _authenticationInfoService;

        public StartupRoute(
            IMainRoute mainRoute,
            IOnBoardingRoute onboardingRoute,
            ICreateProfileRoute createProfileRoute,
            IAuthenticationInfoService authenticationInfoService)
        {
            _mainRoute = mainRoute;
            _onboardingRoute = onboardingRoute;
            _createProfileRoute = createProfileRoute;
            _authenticationInfoService = authenticationInfoService;
        }

        public async Task ExecuteAsync(INavigationService navigationService)
        {
            await ShowFirstPageAsync(navigationService);
        }

        private Task ShowFirstPageAsync(INavigationService navigationService)
        {
            if (_authenticationInfoService.IsAuthenticated())
            {
                if (Xamarin.Essentials.Preferences.Get("OnBoarded", false))
                {
                    return _mainRoute.ExecuteAsync(navigationService);
                }
                else
                {
                    return _onboardingRoute.ExecuteAsync(navigationService);
                }
            }
            else
            {
                return _createProfileRoute.ExecuteAsync(navigationService);
            }
        }
    }
}
