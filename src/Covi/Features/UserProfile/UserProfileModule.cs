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

using Covi.Features.UserProfile.Components.BluetoothReminder;
using Covi.Features.UserProfile.Components.ChangeStatus;
using Covi.Features.UserProfile.Components.EncryptionStatus;
using Covi.Features.UserProfile.Components.HealthStatusAdvice;
using Covi.Features.UserProfile.Components.UserStatusCard;
using Prism.Ioc;
using Prism.Modularity;

namespace Covi.Features.UserProfile
{
    public class UserProfileModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<Features.UserProfile.UserProfilePage, Features.UserProfile.UserProfileViewModel>();

            containerRegistry.Register<UserStatusCardViewModel>();
            containerRegistry.Register<ChangeStatusViewModel>();
            containerRegistry.Register<HealthStatusAdviceViewModel>();
            containerRegistry.Register<EncryptionStatusViewModel>();
            containerRegistry.Register<BluetoothReminderViewModel>();

            containerRegistry.RegisterSingleton<Features.UserProfile.Services.IUserService, Features.UserProfile.Services.UserService>();
            containerRegistry.RegisterSingleton<Features.UserProfile.Services.IUserProfileContainer, Features.UserProfile.Services.UserProfileContainer>();
        }
    }
}
