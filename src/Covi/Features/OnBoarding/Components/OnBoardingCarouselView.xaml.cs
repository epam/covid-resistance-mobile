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
using Xamarin.Forms;

namespace Covi.Features.OnBoarding.Components
{
    public partial class OnBoardingCarouselView : CarouselView
    {
        public IList<OnBoardingItemViewModel> OnBoardingItems
        {
            get => (IList<OnBoardingItemViewModel>)GetValue(OnBoardingItemsProperty);
            set => SetValue(OnBoardingItemsProperty, value);
        }

        public int SelectedItemIndex
        {
            get => (int)GetValue(SelectedItemIndexProperty);
            set => SetValue(SelectedItemIndexProperty, value);
        }

        public static readonly BindableProperty OnBoardingItemsProperty = BindableProperty.Create(
            nameof(OnBoardingItems),
            typeof(IList<OnBoardingItemViewModel>),
            typeof(OnBoardingCarouselView),
            default(IList<OnBoardingItemViewModel>),
            BindingMode.TwoWay);

        public static readonly BindableProperty SelectedItemIndexProperty = BindableProperty.Create(
            nameof(SelectedItemIndex),
            typeof(int),
            typeof(OnBoardingCarouselView),
            default(int),
            BindingMode.OneWay,
            propertyChanged: (bindable, oldValue, newValue) =>
                ((OnBoardingCarouselView)bindable).UpdateSelectedItem((int)newValue));

        public OnBoardingCarouselView()
        {
            InitializeComponent();
        }

        private void UpdateSelectedItem(int newIndex)
        {
            OnBoardingCarousel.ScrollTo(newIndex);
        }
    }
}
