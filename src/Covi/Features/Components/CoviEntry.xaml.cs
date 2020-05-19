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

using System.Windows.Input;
using Xamarin.Forms;

namespace Covi.Features.Components
{
    public partial class CoviEntry : ContentView
    {
        private const string ShowPasswordImageName = "show_password.svg";
        private const string HidePasswordImageName = "hide_password.svg";

        public CoviEntry()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextProperty = BindableProperty.Create(
            nameof(Text),
            typeof(string),
            typeof(CoviEntry),
            string.Empty);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
            nameof(Placeholder),
            typeof(string),
            typeof(CoviEntry),
            string.Empty);

        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }

        public static readonly BindableProperty ShowPasswordEyeProperty = BindableProperty.Create(
            nameof(ShowPasswordEye),
            typeof(bool),
            typeof(CoviEntry),
            false);

        public bool ShowPasswordEye
        {
            get => (bool)GetValue(ShowPasswordEyeProperty);
            set => SetValue(ShowPasswordEyeProperty, value);
        }

        public static readonly BindableProperty HidePasswordProperty = BindableProperty.Create(
            nameof(HidePassword),
            typeof(bool),
            typeof(CoviEntry),
            false);

        public bool HidePassword
        {
            get => (bool)GetValue(HidePasswordProperty);
            set => SetValue(HidePasswordProperty, value);
        }

        public static readonly BindableProperty HasAnErrorProperty = BindableProperty.Create(
            nameof(HasAnError),
            typeof(bool?),
            typeof(CoviEntry),
            false,
            propertyChanging: HasAnErrorPropertyChanged);

        private static void HasAnErrorPropertyChanged(Xamarin.Forms.BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CoviEntry label)
            {
                label.UpdateUnderlineColor((bool?)newValue);
            }
        }

        private void UpdateUnderlineColor(bool? hasError)
        {
            if (hasError.HasValue == false)
            {
                EntryUnderline.BackgroundColor = UnderlineNormalColor;
            }
            else
            {
                EntryUnderline.BackgroundColor = hasError.Value ? UnderlineErrorColor : UnderlineCorrectColor;
            }
        }

        public bool HasAnError
        {
            get => (bool)GetValue(HasAnErrorProperty);
            set => SetValue(HasAnErrorProperty, value);
        }

        public static readonly BindableProperty ErrorTextProperty = BindableProperty.Create(
            nameof(ErrorText),
            typeof(string),
            typeof(CoviEntry),
            string.Empty,
            propertyChanged: ErrorTextPropertyChanged);

        private static void ErrorTextPropertyChanged(Xamarin.Forms.BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CoviEntry entry)
            {
                entry.ErrorTextChangedHandler((string)newValue);
            }
        }

        private void ErrorTextChangedHandler(string value)
        {
            ErrorTextLabel.Text = value;
            ErrorTextLabel.IsVisible = ErrorTextControl.IsVisible = !string.IsNullOrEmpty(value);
        }

        public string ErrorText
        {
            get => (string)GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

        public Color UnderlineErrorColor
        {
            get => (Color)GetValue(UnderlineErrorColorProperty);
            set => SetValue(UnderlineErrorColorProperty, value);
        }

        public static readonly BindableProperty UnderlineErrorColorProperty = BindableProperty.Create(
            nameof(UnderlineErrorColor),
            typeof(Color),
            typeof(CoviEntry), Color.Red);

        public Color UnderlineNormalColor
        {
            get => (Color)GetValue(UnderlineNormalColorProperty);
            set => SetValue(UnderlineNormalColorProperty, value);
        }

        public static readonly BindableProperty UnderlineNormalColorProperty = BindableProperty.Create(
            nameof(UnderlineNormalColor),
            typeof(Color),
            typeof(CoviEntry), Color.Red);

        public Color UnderlineCorrectColor
        {
            get => (Color)GetValue(UnderlineCorrectColorProperty);
            set => SetValue(UnderlineCorrectColorProperty, value);
        }

        public static readonly BindableProperty UnderlineCorrectColorProperty = BindableProperty.Create(
            nameof(UnderlineCorrectColor),
            typeof(Color),
            typeof(CoviEntry), Color.Red);

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            HidePassword = !HidePassword;

            var width = PasswordEye.Width;
            var height = PasswordEye.Height;
            PasswordEye.Source = ImageSource.FromFile(HidePassword ? ShowPasswordImageName : HidePasswordImageName);
            PasswordEye.WidthRequest = width;
            PasswordEye.HeightRequest = height;
        }

        public ReturnType ReturnType
        {
            get => (ReturnType)GetValue(ReturnTypeProperty);
            set => SetValue(ReturnTypeProperty, value);
        }

        public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(
            nameof(ReturnType),
            typeof(ReturnType),
            typeof(CoviEntry), ReturnType.Default);

        public ICommand ReturnCommand
        {
            get => (ICommand)GetValue(ReturnCommandProperty);
            set => SetValue(ReturnCommandProperty, value);
        }

        public static readonly BindableProperty ReturnCommandProperty = BindableProperty.Create(
            nameof(ReturnCommand),
            typeof(ICommand),
            typeof(CoviEntry), null);

        public object ReturnCommandParameters
        {
            get => (object)GetValue(ReturnCommandParametersProperty);
            set => SetValue(ReturnCommandParametersProperty, value);
        }

        public static readonly BindableProperty ReturnCommandParametersProperty = BindableProperty.Create(
            nameof(ReturnCommandParameters),
            typeof(object),
            typeof(CoviEntry), default);
    }
}
