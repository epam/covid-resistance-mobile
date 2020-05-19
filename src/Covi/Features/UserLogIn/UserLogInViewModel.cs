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
using System.Reactive;
using System.Threading.Tasks;
using Covi.Features.Account;
using Covi.Features.Account.Services.SignIn;
using Covi.Features.Exceptions;
using Covi.Features.OnBoarding.Routes;
using Covi.Resources;
using Covi.Services.ErrorHandlers;
using Prism.Navigation;
using ReactiveUI;

namespace Covi.Features.UserLogIn
{
    public class UserLogInViewModel : ViewModelBase
    {
        private string _userName;
        private string _userPassword;
        private readonly INavigationService _navigationService;
        private readonly IOnBoardingRoute _onBoardingRoute;
        private readonly IErrorHandler _errorHandler;
        private readonly ISignInService _singInService;

        public ReactiveCommand<Unit, Unit> LogInCommand { get; }

        public UserLogInViewModel(
            INavigationService navigationService,
            IOnBoardingRoute onBoardingRoute,
            IErrorHandler errorHandler,
            ISignInService singInService)
        {
            _navigationService = navigationService;
            _onBoardingRoute = onBoardingRoute;
            _errorHandler = errorHandler;
            _singInService = singInService;

            var canExecuteLogin = this.WhenAnyValue(
                vm => vm.HasUsernameErrorMessage,
                vm => vm.HasPasswordErrorMessage,
                vm => vm.IsBusy,
                ValidateCanExecuteLogin);

            LogInCommand = ReactiveCommand.CreateFromTask(HandleLogInAsync, canExecuteLogin);
        }

        private static bool ValidateCanExecuteLogin(bool? hasUserNameError, bool? hasPasswordError, bool isBusy)
        {
            bool isUserNameInvalid = hasUserNameError.HasValue && hasUserNameError.Value;
            bool isPasswordInvalid = hasPasswordError.HasValue && hasPasswordError.Value;

            return !isUserNameInvalid && !isPasswordInvalid && !isBusy;
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                ValidateUsername(value);
                this.RaisePropertyChanged();
            }
        }

        public string UserPassword
        {
            get => _userPassword;
            set
            {
                _userPassword = value;
                ValidatePassword(value);
                this.RaisePropertyChanged();
            }
        }

        private void ValidateUsername(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                HasUsernameErrorMessage = true;
                UsernameErrorText = Localization.UserLogIn_Username_EmptyErrorMessage_Text;
            }
            else
            {
                HasUsernameErrorMessage = false;
                UsernameErrorText = string.Empty;
            }
        }

        private void ValidatePassword(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                HasPasswordErrorMessage = true;
                PasswordErrorText = Localization.UserLogIn_Password_EmptyErrorMessage_Text;
            }
            else
            {
                HasPasswordErrorMessage = false;
                PasswordErrorText = string.Empty;
            }
        }

        public bool? HasUsernameErrorMessage { get; private set; }

        public bool? HasPasswordErrorMessage { get; private set; }

        public string UsernameErrorText { get; set; }

        public string PasswordErrorText { get; set; }

        private async Task HandleLogInAsync()
        {
            try
            {
                if (HasPasswordErrorMessage.HasValue && HasPasswordErrorMessage.Value == false &&
                    HasUsernameErrorMessage.HasValue && HasUsernameErrorMessage.Value == false)
                {
                    IsBusy = true;
                    await _singInService.SignInAsync(
                        new UserCredentials(UserName, UserPassword));

                    Xamarin.Essentials.Preferences.Set("SignedIn", true);
                    await _onBoardingRoute.ExecuteAsync(_navigationService);
                }
            }
            catch (LoginOperationException e)
            {
                PasswordErrorText = e.Message;
            }
            catch (Exception e)
            {
                await _errorHandler.HandleAsync(e);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
