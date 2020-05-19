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
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.TracingInformation;
using Covi.Features.Main.Routes;
using Covi.Resources;
using Covi.Services.Notification;
using Covi.Services.Security.SecretsProvider;
using Prism.Navigation;
using ReactiveUI;
using Xamarin.Forms;

namespace Covi.Features.OnBoarding
{
    public class OnBoardingViewModel : ComponentViewModelBase
    {
        private const string TailoredForYouImage = "tailored_for_you.svg";
        private const string PrivacyImage = "completely_anonymous.svg";
        private const string ConnectivityImage = "connect_with_bluetooth.svg";
        private const string NotificationsImage = "important_notifications.svg";

        private readonly INotificationManager _notificationManager;
        private readonly ITracingEngine _tracingEngine;
        private readonly INavigationService _navigationService;
        private readonly IMainRoute _mainRoute;
        private readonly ISecretsProvider _secretsProvider;

        public int SelectedActivityIndex { get; set; }
        public IList<OnBoardingItemViewModel> OnBoardingActivities { get; private set; }
        public int ActivitiesCount { get; private set; }
        public ReactiveCommand<Unit, Unit> GoToNextStepCommand { get; }

        public OnBoardingViewModel(
            INavigationService navigationService,
            IMainRoute mainRoute,
            INotificationManager notificationManager,
            ITracingEngine tracingEngine,
            ISecretsProvider secretsProvider)
        {
            GoToNextStepCommand = ReactiveUI.ReactiveCommand.CreateFromTask(HandleNextStepAsync);

            _navigationService = navigationService;
            _mainRoute = mainRoute;
            _notificationManager = notificationManager;
            _tracingEngine = tracingEngine;
            _secretsProvider = secretsProvider;
            SetInstructions();
        }

        private async Task HandleNextStepAsync()
        {
            await ShowNotificationIfNecessary(ActivityType.Connectivity, ShowBluetoothNotification);

            if (SelectedActivityIndex == OnBoardingActivities.Count)
            {
                Xamarin.Essentials.Preferences.Set("OnBoarded", true);
                await _mainRoute.ExecuteAsync(_navigationService);
            }
        }

        private async Task ShowNotificationIfNecessary(ActivityType activityType, Func<Task<bool>> action)
        {
            IsBusy = true;
            var isNecessaryStep = OnBoardingActivities[SelectedActivityIndex].Type == activityType;

            if (isNecessaryStep)
            {
                if (await action.Invoke())
                {
                    SelectedActivityIndex++;
                }
            }
            else
            {
                SelectedActivityIndex++;
            }
            IsBusy = false;
        }

        private async Task<bool> ShowBluetoothNotification()
        {
            bool accepted = false;
            string errorMessage = string.Empty;
            try
            {
                var result = await InitBluetoothAsync();
                switch (result.PermissionStatus)
                {
                    case Xamarin.Essentials.PermissionStatus.Disabled:
                        await _notificationManager.ShowNotificationAsync(
                            Localization.BluetoothNotification_Error_Title,
                            Localization.BluetoothNotification_Disabled_ErrorText,
                            Localization.ErrorDialog_Accept);
                        break;
                    case Xamarin.Essentials.PermissionStatus.Denied:
                        await _notificationManager.ShowNotificationAsync(
                            Localization.BluetoothNotification_Error_Title,
                            Localization.BluetoothNotification_Denied_ErrorText,
                            Localization.ErrorDialog_Accept);
                        break;
                    case Xamarin.Essentials.PermissionStatus.Granted:
                        accepted = true;
                        break;
                }

                if (accepted)
                {
                    await _tracingEngine.StartAsync();
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                await _notificationManager.ShowNotificationAsync(
                    Localization.BluetoothNotification_Error_Title,
                    errorMessage,
                    Localization.ErrorDialog_Accept);
                accepted = false;
            }

            return accepted;
        }

        private async Task<InitializationResult> InitBluetoothAsync()
        {
            var deviceId = await _secretsProvider.GetDeviceIdentifierAsync().ConfigureAwait(false);
            var result = await _tracingEngine.SetupAsync(new TracingInformation(true, deviceId, Configuration.Constants.BluetoothConstants.ServiceUUID, Configuration.Constants.BluetoothConstants.CharacteristicUUID, Configuration.Constants.BluetoothConstants.DataExpirationTime));
            if (!result.Success)
            {
                return result;
            }

            return result;
        }

        private void SetInstructions()
        {
            OnBoardingActivities = new List<OnBoardingItemViewModel>
            {
                new OnBoardingItemViewModel
                {
                    Type = ActivityType.MadeForYou,
                    Title = Localization.OnBoarding_MadeForYou_Title.ToUpper(),
                    SubTitle = Localization.OnBoardind_MadeForYou_SubTitle,
                    IconCode = TailoredForYouImage,
                    Instructions = new List<InstructionItemViewModel>
                    {
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_MadeForYou_Instruction1},
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_MadeForYou_Instruction2},
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_MadeForYou_Instruction3}
                    }
                },
                new OnBoardingItemViewModel
                {
                    Type = ActivityType.Privacy,
                    Title = Localization.OnBoarding_Privacy_Title.ToUpper(),
                    SubTitle = Localization.OnBoarding_Privacy_SubTitle,
                    IconCode = PrivacyImage,
                    Instructions = new List<InstructionItemViewModel>
                    {
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Privacy_Instruction1},
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Privacy_Instruction2},
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Privacy_Instruction3}
                    }
                },
                new OnBoardingItemViewModel
                {
                    Type = ActivityType.Connectivity,
                    Title = Localization.OnBoarding_Connectivity_Title.ToUpper(),
                    SubTitle = Localization.OnBoarding_Connectivity_SubTitle,
                    IconCode = ConnectivityImage,
                    ErrorMessage = Localization.OnBoarding_Connectivity_ErrorText,
                    Instructions = ConnectivityInstructions()
                },
                new OnBoardingItemViewModel
                {
                    Type = ActivityType.Notifications,
                    Title = Localization.OnBoarding_Notifications_Title.ToUpper(),
                    SubTitle = Localization.OnBoarding_Notifications_SubTitle,
                    IconCode = NotificationsImage,
                    ErrorMessage = Localization.OnBoarding_Notifications_ErrorText,
                    Instructions = new List<InstructionItemViewModel>
                    {
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Notifications_Instruction1},
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Notifications_Instruction2},
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Notifications_Instruction3},
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Notifications_Instruction4}
                    }
                }
            };

            ActivitiesCount = OnBoardingActivities.Count;
        }

        private List<InstructionItemViewModel> ConnectivityInstructions()
        {
            var instructions = new List<InstructionItemViewModel>
                    {
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Connectivity_Instruction1},
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Connectivity_Instruction2},
                        new InstructionItemViewModel{InstructionText = Localization.OnBoarding_Connectivity_Instruction3}
                    };

            if (Device.RuntimePlatform == Device.Android)
            {
                instructions.Add(
                    new InstructionItemViewModel { InstructionText = Localization.OnBoarding_Connectivity_Instruction4_AndroidOnly });
            }

            return instructions;
        }
    }
}
