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
using Xamarin.Forms;

namespace Covi.Features.MedicalChangeStatus.Components
{
    public partial class DatePickerView : ContentView
    {
        public DateTime SelectedDate
        {
            get => (DateTime)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly BindableProperty SelectedDateProperty =
            BindableProperty.Create(
                nameof(SelectedDate),
                typeof(DateTime),
                typeof(DatePickerView),
                default(DateTime),
                defaultBindingMode: BindingMode.TwoWay);

        public DatePickerView()
        {
            InitializeComponent();
        }

        protected void DateBtnClicked(object sender, EventArgs e)
        {
            DatePicker.Focus();
        }

        protected void DatePickerDateSelected(object sender, DateChangedEventArgs e)
        {
            SelectedDate = e.NewDate;
        }
    }
}
