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
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Covi.Features.MedicalCodeSharing.Components.SharedCode
{
    public partial class SharedCodeView : ContentView
    {
        private static TimeSpan _animationDuration = TimeSpan.FromMilliseconds(500);
        private EventHandler _handler;

        public SharedCodeView()
        {
            BindingContextChanged += HandleContextChanged;
            InitializeComponent();
            Prism.Mvvm.ViewModelLocator.SetAutowireViewModel(this, true);
        }

        private void HandleContextChanged(object sender, EventArgs e)
        {
            if (BindingContext is SharedCodeViewModel codeGeneratorViewModel)
            {
                _handler = CodeGeneratorViewModelOnCodeCopied;
                codeGeneratorViewModel.CodeCopied -= _handler;
                codeGeneratorViewModel.CodeCopied += _handler;
            }
        }

        private async void CodeGeneratorViewModelOnCodeCopied(object sender, EventArgs e)
        {
            CopyPopUp.IsVisible = true;
            await Task.Delay(_animationDuration);
            await CopyPopUp.FadeTo(0, (uint)_animationDuration.TotalMilliseconds);
            CopyPopUp.IsVisible = false;
            CopyPopUp.Opacity = 1;
        }
    }
}
