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

using System.Reactive.Disposables;
using Covi.Features.PushNotifications.Services;
using Covi.Features.UserProfile.Services;
using Prism.Navigation;
using System.Threading.Tasks;
using Covi.Features.AppSettings;

namespace Covi.Features.UserProfile
{
    public class UserProfileViewModel : CompositeViewModelBase
    {
        private readonly IUserService _userService;
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly IAppPreferencesService _appStateContainer;

        public UserProfileViewModel(
            INavigationService navigationService,
            IUserService userService,
            IPushNotificationsService pushNotificationsService,
            IAppPreferencesService appStateContainer)
            : base(navigationService)
        {
            _userService = userService;
            _pushNotificationsService = pushNotificationsService;
            _appStateContainer = appStateContainer;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            _pushNotificationsService.UpdateTokenAsync().FireAndForget();
            _userService.GetUserStateAsync().FireAndForget();
            _appStateContainer.IsAppNeedsForceRefreshUserStatus = false;
        }

        public override void OnActivated(CompositeDisposable lifecycleDisposable)
        {
            base.OnActivated(lifecycleDisposable);

            if (_appStateContainer.IsAppNeedsForceRefreshUserStatus)
            {
                RequestUserStateUpdateAsync().FireAndForget();
            }
        }

        private async Task RequestUserStateUpdateAsync()
        {
            try
            {
                IsBusy = true;

                _appStateContainer.IsAppNeedsForceRefreshUserStatus = false;
                await _userService.GetUserStateAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
