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

namespace Covi.Features.OnBoarding.Components
{
    public partial class HorizontalSelectedItemIndicatorControl
    {
        private const double _defaultIndicatorWidth = 16;

        public int ItemCount
        {
            get => (int) GetValue(ItemCountProperty);
            set => SetValue(ItemCountProperty, value);
        }

        public int SelectedItemIndex
        {
            get => (int) GetValue(SelectedItemIndexProperty);
            set => SetValue(SelectedItemIndexProperty, value);
        }

        public Color IndicatorColor
        {
            get => (Color) GetValue(IndicatorColorProperty);
            set => SetValue(IndicatorColorProperty, value);
        }

        public static readonly BindableProperty ItemCountProperty = BindableProperty.Create(
            nameof(ItemCount),
            typeof(int),
            typeof(HorizontalSelectedItemIndicatorControl),
            default(int),
            BindingMode.OneWay,
            validateValue: (bindable, value) => (int) value >= 0,
            propertyChanged: (bindable, oldValue, newValue) =>
                ((HorizontalSelectedItemIndicatorControl) bindable).RecreateIndicators());

        public static readonly BindableProperty SelectedItemIndexProperty = BindableProperty.Create(
            nameof(SelectedItemIndex),
            typeof(int),
            typeof(HorizontalSelectedItemIndicatorControl),
            default(int),
            BindingMode.OneWay,
            propertyChanged: (bindable, oldValue, newValue) =>
                ((HorizontalSelectedItemIndicatorControl) bindable).UpdateSelectedItem((int) oldValue, (int) newValue));

        public static readonly BindableProperty IndicatorColorProperty = BindableProperty.Create(
            nameof(IndicatorColor),
            typeof(Color),
            typeof(HorizontalSelectedItemIndicatorControl), Color.DarkBlue,
            propertyChanged: (bindable, oldValue, newValue) =>
                ((HorizontalSelectedItemIndicatorControl) bindable).UpdateIndicatorsColor());

        public HorizontalSelectedItemIndicatorControl()
        {
            InitializeComponent();
            RecreateIndicators();
        }

        private void UpdateSelectedItem(int oldIndex, int newIndex)
        {
            if (ItemCount == 0 || newIndex >= ItemCount || newIndex < 0)
            {
                return;
            }

            if (oldIndex >= 0 && oldIndex <= ContainerStackLayout.Children.Count - 1)
            {
                ((Frame) ContainerStackLayout.Children[oldIndex]).BackgroundColor = Color.Transparent;
            }

            ((Frame) ContainerStackLayout.Children[newIndex]).BackgroundColor = IndicatorColor;
        }

        private void UpdateIndicatorsColor()
        {
            foreach (var child in ContainerStackLayout.Children)
            {
                child.BackgroundColor = IndicatorColor;
            }
        }

        private void RecreateIndicators()
        {
            ContainerStackLayout.Children.Clear();
            ContainerStackLayout.RowDefinitions.Clear();
            ContainerStackLayout.ColumnDefinitions.Clear();

            ContainerStackLayout.RowDefinitions.Add(new RowDefinition {Height = _defaultIndicatorWidth});

            for (var i = 0; i < ItemCount; i++)
            {
                ContainerStackLayout.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = _defaultIndicatorWidth * 2
                });
                ContainerStackLayout.Children.Add(CreateIndicator(i == SelectedItemIndex),
                    ContainerStackLayout.ColumnDefinitions.Count - 1, 0);
            }
        }

        private View CreateIndicator(bool isSelected)
        {
            return new Frame
            {
                HasShadow = false,
                BorderColor = IndicatorColor,
                WidthRequest = _defaultIndicatorWidth,
                HeightRequest = _defaultIndicatorWidth,
                CornerRadius = (float)(_defaultIndicatorWidth / 2),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BackgroundColor = isSelected ? IndicatorColor : Color.Transparent,
                Margin = new Thickness(_defaultIndicatorWidth / 2, 0)
            };
        }
    }
}
