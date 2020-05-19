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
using Covi.Features.ApplyCode.Services;
using Covi.Features.Main.Routes;
using Covi.Resources;
using Covi.Services.ErrorHandlers;
using ReactiveUI;

namespace Covi.Features.ApplyCode
{
    public class ApplyCodeViewModel : ViewModelBase
    {
        private readonly Prism.Navigation.INavigationService _navigationService;
        private readonly IMainRoute _mainRoute;
        private readonly IUserStatusChangeService _userStatusChangeService;
        private readonly IErrorHandler _errorHandler;

        private string _codeValue;

        public string CodeValue
        {
            get => _codeValue;
            set
            {
                _codeValue = value;
                ValidateCode(value);
                this.RaisePropertyChanged(CodeValue);
            }
        }

        public bool? HasCodeValueErrorMessage { get; private set; }

        public string CodeValueErrorText { get; set; }

        public ReactiveCommand<Unit, Unit> ApplyCodeCommand { get; }

        public ApplyCodeViewModel(
            Prism.Navigation.INavigationService navigationService,
            IMainRoute mainRoute,
            IUserStatusChangeService userStatusChangeService,
            IErrorHandler errorHandler)
        {
            _mainRoute = mainRoute;
            _userStatusChangeService = userStatusChangeService;
            _navigationService = navigationService;
            _errorHandler = errorHandler;

            var canApplyCode = this.WhenAnyValue(
                vm => vm.IsBusy,
                x => !x);

            ApplyCodeCommand = ReactiveCommand.CreateFromTask(HandleApplyCodeCommandAsync, canApplyCode);
        }

        private void ValidateCode(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                HasCodeValueErrorMessage = true;
                CodeValueErrorText = Localization.ApplyCode_InputError_Text;
            }
            else
            {
                HasCodeValueErrorMessage = false;
                CodeValueErrorText = string.Empty;
            }
        }

        private async Task HandleApplyCodeCommandAsync()
        {
            try
            {
                IsBusy = true;
                var code = CodeValue;

                if (string.IsNullOrWhiteSpace(code))
                {
                    return;
                }

                var userProfile = await _userStatusChangeService.ApplyStatusChangeCode(code);

                if (userProfile != null)
                {
                    await _mainRoute.ExecuteAsync(_navigationService);
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
}
