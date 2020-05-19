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
using Covi.Resources;
using Covi.Services.ErrorHandlers;
using Prism.Navigation;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Covi.Features.MedicalCodeSharing.Components.ShareButton
{
    public class ShareButtonViewModel : ComponentViewModelBase
    {
        private readonly IErrorHandler _errorHandler;
        private string _code;

        public Command ShareCodeCommand { get; }

        public ShareButtonViewModel(IErrorHandler errorHandler)
        {
            _errorHandler = errorHandler;
            ShareCodeCommand = new Command(ShareCode);
        }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);

            if (parameters.TryGetValue(Routes.MedicalCodeSharingSharingRoute.MedicalCodeFieldName, out string code))
            {
                _code = code;
            }
        }

        private async void ShareCode()
        {
            try
            {
                await Share.RequestAsync(new ShareTextRequest(_code, Localization.MedicalCodeSharing_ShareSystem_TitleText));
            }
            catch (Exception ex)
            {
                await _errorHandler.HandleAsync(ex);
            }
        }
    }
}
