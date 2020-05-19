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
using Covi.Features.MedicalLogin.Services;
using Covi.Resources;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;
using Covi.Services.ErrorHandlers;

namespace Covi.Features.MedicalLogin.Components.LogIn
{
    public class MedicalLoginViewModel : ComponentViewModelBase
    {
        private string _medicalId;
        private string _medicalIdErrorText;
        private readonly IMedicalAuthenticationService _medicalAuthenticationService;
        private readonly IErrorHandler _errorHandler;

        public MedicalLoginViewModel(
            IMedicalAuthenticationService medicalAuthenticationService,
            IErrorHandler errorHandler)
        {
            _medicalAuthenticationService = medicalAuthenticationService;
            _errorHandler = errorHandler;

            var canExecuteMedicalLogIn = this.WhenAnyValue(
                vm => vm.IsBusy,
                (x) => !x);

            MedicalEnterIdCommand = ReactiveCommand.CreateFromTask(HandleMedicalLogIn, canExecuteMedicalLogIn);
        }

        public ReactiveCommand<Unit, Unit> MedicalEnterIdCommand { get; }

        public string MedicalId
        {
            get => _medicalId;
            set
            {
                _medicalId = value;
                ValidateMedicalId(value);
                this.RaisePropertyChanged();
            }
        }

        private void ValidateMedicalId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                HasMedicalIdErrorMessage = true;
                MedicalIdErrorText = Localization.MedicalLogIn_MedicalId_EmptyErrorMessage_Text;
            }
            else
            {
                HasMedicalIdErrorMessage = false;
                MedicalIdErrorText = string.Empty;
            }
        }

        public string MedicalIdErrorText
        {
            get => _medicalIdErrorText;
            set
            {
                _medicalIdErrorText = value;
                this.RaisePropertyChanged();
            }
        }

        public bool? HasMedicalIdErrorMessage { get; private set; }

        private async Task HandleMedicalLogIn()
        {
            try
            {
                if (HasMedicalIdErrorMessage.HasValue && HasMedicalIdErrorMessage.Value == false)
                {
                    IsBusy = true;
                    if (!string.IsNullOrEmpty(MedicalId))
                    {
                        await _medicalAuthenticationService.MedicalAuthenticateAsync(MedicalId);
                    }
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
