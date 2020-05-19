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

using Xamarin.Forms;

namespace Covi.Features.Components
{
    public partial class RoundCheckBox : ContentView
    {
        private const string CircleEmptyImageName = "passwordcircle_empty.svg";
        private const string CircleCheckedImageName = "passwordcircle_tick.svg";
        private const string CircleCrossedImageName = "passwordcircle_cross.svg";

        public RoundCheckBox()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(RoundCheckBox),
            string.Empty);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
            nameof(TextColor),
            typeof(Color),
            typeof(RoundCheckBox),
            Color.White);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        public static readonly BindableProperty ValidStatusProperty = BindableProperty.Create(
            nameof(ValidStatus),
            typeof(bool?),
            typeof(RoundCheckBox),
            null,
            propertyChanged: Prop);

        private static void Prop(Xamarin.Forms.BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is RoundCheckBox box)
            {
                box.ChangeValidationStatus((bool?)newValue);
            }
        }

        public bool? ValidStatus
        {
            get => (bool?)GetValue(ValidStatusProperty);
            set => SetValue(ValidStatusProperty, value);
        }

        public void ChangeValidationStatus(bool? status)
        {
            if (status == null)
            {
                SetNewSvgImage(CircleEmptyImageName);
                return;
            }

            SetNewSvgImage(status.Value ? CircleCheckedImageName : CircleCrossedImageName);
        }

        private void SetNewSvgImage(string svgImageName)
        {
            var width = RoundCheckBoxImage.Width;
            var height = RoundCheckBoxImage.Height;
            RoundCheckBoxImage.Source = ImageSource.FromFile(svgImageName);
            RoundCheckBoxImage.WidthRequest = width;
            RoundCheckBoxImage.HeightRequest = height;
        }
    }
}
