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

namespace Covi.Features.UserProfile
{
    public partial class UserProfilePage : ContentPage
    {
        public UserProfilePage()
        {
            InitializeComponent();
            this.BindingContextChanged += HandleBindingContextChanged;
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
                compositeViewModel.AttachComponent(this.infectedStatusNotificationView.BindingContext as IComponent);
                compositeViewModel.AttachComponent(this.userStatusCardView.BindingContext as IComponent);
                compositeViewModel.AttachComponent(this.encryption.BindingContext as IComponent);
                compositeViewModel.AttachComponent(this.bluetooth.BindingContext as IComponent);
                compositeViewModel.AttachComponent(this.changeStatus.BindingContext as IComponent);
            }
        }
    }
}
