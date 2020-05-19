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
using System.Collections.Generic;
using System.Linq;
using Prism.Logging;

namespace Covi.Logs
{
    /// <summary>
    /// Prism logger that logs errors into console.
    /// </summary>
    public class PrismConsoleLogger : Prism.Logging.ILogger
    {
        public void Log(string message, IDictionary<string, string> properties)
        {
            var parms = ConvertDictionaryToString(properties);
            System.Console.WriteLine($"Log: {message}.{Environment.NewLine}{parms}");
        }

        public void Log(string message, Category category, Priority priority)
        {
            System.Console.WriteLine($"Log: {category.ToString()}: {message}");
        }

        public void Report(Exception ex, IDictionary<string, string> properties)
        {
            var parms = ConvertDictionaryToString(properties);
            System.Console.WriteLine($"Exception: {ex.ToString()}.{Environment.NewLine}{parms}");
        }

        public void TrackEvent(string name, IDictionary<string, string> properties)
        {
            var parms = ConvertDictionaryToString(properties);
            System.Console.WriteLine($"Event: {name}.{Environment.NewLine}{parms}");
        }

        private string ConvertDictionaryToString(IDictionary<string, string> properties)
        {
            if (properties?.Any() == false)
            {
                return string.Empty;
            }

            return string.Join(",", properties.Select((kvp) => $"{kvp.Key}:{kvp.Value}"));
        }
    }
}
