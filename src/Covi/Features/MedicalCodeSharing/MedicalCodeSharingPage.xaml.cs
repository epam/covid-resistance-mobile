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
using Covi.PlatformSpecific;
using Xamarin.Forms;

namespace Covi.Features.MedicalCodeSharing
{
    public partial class MedicalCodeSharingPage : ContentPage
    {
        public MedicalCodeSharingPage()
        {
            InitializeComponent();
            BindingContextChanged += HandleBindingContextChanged;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            DependencyService.Get<IStatusBarPlatformSpecific>().SetStatusBarColor(Header.BackgroundColor);
        }

        private void HandleBindingContextChanged(object sender, EventArgs e)
        {
            if (BindingContext is CompositeViewModelBase compositeViewModel)
            {
                compositeViewModel.AttachComponent(titleGeneratorView.BindingContext as IComponent);
                compositeViewModel.AttachComponent(shareButtonView.BindingContext as IComponent);
                compositeViewModel.AttachComponent(shareCodeView.BindingContext as IComponent);
            }
        }
    }
}
