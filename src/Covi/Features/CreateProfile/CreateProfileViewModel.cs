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
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Covi.Features.Account.Services.SignUp;
using Covi.Features.CreateProfile.Validation;
using Covi.Features.Exceptions;
using Covi.Features.OnBoarding.Routes;
using Covi.Features.UserLogIn.Routes;
using Covi.Services.ErrorHandlers;
using Prism.Navigation;
using ReactiveUI;

namespace Covi.Features.CreateProfile
{
    public class CreateProfileViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IOnBoardingRoute _onBoardingRoute;
        private readonly IUserLogInRoute _userLogInRoute;
        private readonly IErrorHandler _errorHandler;
        private readonly ISignUpService _signUpService;
        private readonly MinLengthRule _minLengthRule = new MinLengthRule();
        private readonly OneUpperCaseRule _oneUpperCaseRule = new OneUpperCaseRule();
        private readonly OneLowerCaseRule _oneLowerCaseRule = new OneLowerCaseRule();
        private readonly OneSpecialSymbolRule _oneSpecialSymbolRule = new OneSpecialSymbolRule();
        private string _usernameValue;
        private string _passValue;
        private string _passConfirmValue;

        public CreateProfileViewModel(
            INavigationService navigationService,
            IOnBoardingRoute onBoardingRoute,
            IUserLogInRoute userLogInRoute,
            IErrorHandler errorHandler,
            ISignUpService signUpService)
        {
            _navigationService = navigationService;
            _onBoardingRoute = onBoardingRoute;
            _userLogInRoute = userLogInRoute;
            _errorHandler = errorHandler;
            _signUpService = signUpService;

            var canExecuteCreateAccount = this.WhenAnyValue(
                vm => vm.IsBusy,
                (x) => !x);

            CreateAccountCommand = ReactiveCommand.CreateFromTask(HandleCreateAccountAsync, canExecuteCreateAccount);
            LogInCommand = ReactiveCommand.CreateFromTask(HandleLogInAsync);
        }

        public ReactiveCommand<Unit, Unit> CreateAccountCommand { get; }

        public ReactiveCommand<Unit, Unit> LogInCommand { get; }

        public bool? MinLengthRuleIsSatisfied { get; private set; }

        public bool? OneUpperCaseRuleIsSatisafied { get; private set; }

        public bool? OneLowerCaseRuleIsSatisafied { get; private set; }

        public bool? OneSpecialSymbolRuleIsSatisafied { get; private set; }

        public bool? UsernameHasAnError { get; private set; }

        public bool? PasswordHasAnError { get; private set; }

        public bool? ConfirmPasswordHasAnError { get; private set; }

        public string ErrorText { get; private set; }

        public string UsernameErrorText { get; private set; }

        public string UsernameValue
        {
            get => _usernameValue;
            set
            {
                _usernameValue = value;
                ValidateUsername();
            }
        }

        public string PasswordValue
        {
            get => _passValue;
            set
            {
                _passValue = value;
                ValidatePassword();
                ValidateConfirmPassword();
            }
        }

        public string PasswordConfirmValue
        {
            get => _passConfirmValue;
            set
            {
                _passConfirmValue = value;
                ValidateConfirmPassword();
            }
        }

        private void ValidateUsername()
        {
            if (string.IsNullOrWhiteSpace(UsernameValue))
            {
                UsernameHasAnError = true;
                UsernameErrorText = Resources.Localization.CreateProfile_Requirement_Filled_Username_Text;
            }
            else
            {
                bool hasNoSpecialSymbols = !_oneSpecialSymbolRule.Validate(UsernameValue).IsValid;
                if (hasNoSpecialSymbols)
                {
                    UsernameHasAnError = false;
                    UsernameErrorText = string.Empty;
                }
                else
                {
                    UsernameHasAnError = true;
                    UsernameErrorText = Resources.Localization
                        .CreateProfileValidationException_Username_HasSpecialSymbolErrorText;
                }
            }
        }

        private void ValidatePassword()
        {
            if (string.IsNullOrEmpty(PasswordValue))
            {
                MinLengthRuleIsSatisfied =
                    OneUpperCaseRuleIsSatisafied =
                    OneLowerCaseRuleIsSatisafied =
                    OneSpecialSymbolRuleIsSatisafied =
                    PasswordHasAnError = default;

                ErrorText = string.Empty;
                return;
            }

            var val1 = _minLengthRule.Validate(PasswordValue);
            var val2 = _oneUpperCaseRule.Validate(PasswordValue);
            var val3 = _oneLowerCaseRule.Validate(PasswordValue);
            var val4 = _oneSpecialSymbolRule.Validate(PasswordValue);

            var isValid = val1.IsValid &&
                val2.IsValid &&
                val3.IsValid &&
                val4.IsValid;

            MinLengthRuleIsSatisfied = val1.IsValid;
            OneUpperCaseRuleIsSatisafied = val2.IsValid;
            OneLowerCaseRuleIsSatisafied = val3.IsValid;
            OneSpecialSymbolRuleIsSatisafied = val4.IsValid;

            PasswordHasAnError = !isValid;
        }

        private void ValidateConfirmPassword()
        {
            var isValid = string.Equals(PasswordValue ?? string.Empty, PasswordConfirmValue ?? string.Empty);

            ErrorText = isValid ? string.Empty : Resources.Localization.CreateProfile_Requirement_Same_Passwords_Text;

            ConfirmPasswordHasAnError = !isValid;
        }

        private async Task HandleCreateAccountAsync()
        {
            if (UsernameHasAnError.HasValue && UsernameHasAnError.Value == false
                && PasswordHasAnError.HasValue && PasswordHasAnError.Value == false
                && ConfirmPasswordHasAnError.HasValue && ConfirmPasswordHasAnError.Value == false)
            {
                try
                {
                    IsBusy = true;
                    await _signUpService.CreateProfileAsync(
                        new UserSignUpInfo(UsernameValue, PasswordValue));
                    await _onBoardingRoute.ExecuteAsync(_navigationService);
                }
                catch (CreateProfileCredentialsValidationException ex)
                {
                    if (ex.UsernameErrors.Any())
                    {
                        var usernameErrors = string.Join(";", ex.UsernameErrors);
                        UsernameHasAnError = !string.IsNullOrWhiteSpace(usernameErrors);
                        UsernameErrorText = usernameErrors;
                    }

                    if (ex.PasswordErrors.Any())
                    {
                        var passwordErrors = string.Join(";", ex.PasswordErrors);
                        PasswordHasAnError = !string.IsNullOrWhiteSpace(passwordErrors);
                        ErrorText = passwordErrors;
                    }
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

        private async Task HandleLogInAsync()
        {
            await _userLogInRoute.ExecuteAsync(_navigationService);
        }
    }
}
