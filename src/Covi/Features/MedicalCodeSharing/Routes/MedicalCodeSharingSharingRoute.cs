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
using Prism.Navigation;

namespace Covi.Features.MedicalCodeSharing.Routes
{
    public class MedicalCodeSharingSharingRoute : IMedicalCodeSharingRoute
    {
        public const string MedicalStatusNameFieldName = nameof(MedicalCodeSharingParameters.MedicalStatusName);
        public const string MedicalCodeFieldName = nameof(MedicalCodeSharingParameters.MedicalCode);

        public async Task ExecuteAsync(INavigationService navigationService, MedicalCodeSharingParameters sharingParameters)
        {
            var navParams = new NavigationParameters
            {
                { MedicalCodeFieldName, sharingParameters.MedicalCode },
                { MedicalStatusNameFieldName, sharingParameters.MedicalStatusName },
            };
            await ExecuteAsync(navigationService, navParams);
        }

        private async Task ExecuteAsync(INavigationService navigationService, INavigationParameters parameters)
        {
            var page = $"{nameof(MedicalCodeSharingPage)}";
            var r = parameters != null
                ? await navigationService.NavigateAsync(page, parameters)
                : await navigationService.NavigateAsync(page);
        }
    }

    public class MedicalCodeSharingParameters
    {
        public MedicalCodeSharingParameters(string medicalStatusName, string medicalCode)
        {
            MedicalStatusName = medicalStatusName;
            MedicalCode = medicalCode;
        }

        public string MedicalStatusName { get; }

        public string MedicalCode { get; }
    }
}
