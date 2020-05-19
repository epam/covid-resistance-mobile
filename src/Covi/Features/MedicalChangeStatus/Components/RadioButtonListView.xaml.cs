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

using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Covi.Features.MedicalChangeStatus.Components
{
    public partial class RadioButtonListView : ContentView
    {

        public IEnumerable<MedicalOptionItemViewModel> OptionsList
        {
            get => (IEnumerable<MedicalOptionItemViewModel>)GetValue(OptionsListProperty);
            set => SetValue(OptionsListProperty, value);
        }

        public static readonly BindableProperty OptionsListProperty = BindableProperty.Create(
            nameof(OptionsList),
            typeof(IEnumerable<MedicalOptionItemViewModel>),
            typeof(RadioButtonListView),
            Enumerable.Empty<MedicalOptionItemViewModel>(),
            BindingMode.TwoWay);

        public RadioButtonListView()
        {
            InitializeComponent();
        }
    }
}
