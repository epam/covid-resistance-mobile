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
using Covi.Services.ErrorHandlers;
using Prism.Navigation;
using ReactiveUI;
using Xamarin.Essentials;

namespace Covi.Features.MedicalCodeSharing.Components.SharedCode
{
    public class SharedCodeViewModel : ComponentViewModelBase
    {
        private readonly IErrorHandler _errorHandler;

        public event EventHandler CodeCopied;

        public string Code { get; private set; }

        public ReactiveCommand<Unit, Unit> CopyCodeCommand { get; }

        public SharedCodeViewModel(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            CopyCodeCommand = ReactiveCommand.CreateFromTask(CopyCodeAsync);
        }

        private async Task CopyCodeAsync()
        {
            try
            {
                await Clipboard.SetTextAsync(Code);
                if (Clipboard.HasText)
                {
                    RaiseOnCodeCopied();
                }
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleAsync(ex);
            }
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(Routes.MedicalCodeSharingSharingRoute.MedicalCodeFieldName, out string code))
            {
                Code = code;
            }
        }

        protected virtual void RaiseOnCodeCopied()
        {
            CodeCopied?.Invoke(this, EventArgs.Empty);
        }
    }
}
