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
using Covi.Features.UserProfile.Components.UserStatusCard;
using Xamarin.Forms;

namespace Covi.Converters
{
    public class HealthStatusColorConverter : IValueConverter
    {
        public Color CriticalSeverityColor { get; set; }

        public Color LowSeverityColor { get; set; }

        public Color HighSeverityColor { get; set; }

        public Color NotRecognizedSeverityColor { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Severity severity)) return NotRecognizedSeverityColor;
            switch (severity)
            {
                case Severity.Low:
                    return LowSeverityColor;
                case Severity.High:
                    return HighSeverityColor;
                case Severity.Critical:
                    return CriticalSeverityColor;
                case Severity.NotRecognized:
                    return NotRecognizedSeverityColor;
                default:
                    return NotRecognizedSeverityColor;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
