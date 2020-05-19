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
using Covi.Configuration;
using Mobile.BuildTools.Configuration;

namespace Covi.iOS.Configuration
{
    public class iOSConfiguration : IEnvironmentConfiguration
    {
        private readonly IConfigurationManager _configurationManager;

        public iOSConfiguration(IConfigurationManager configurationManager)
        {
            _configurationManager = configurationManager;
        }

        public INameValueCollection Configs => _configurationManager.AppSettings;

        public string GetValue(string key)
        {
            return Configs[key];
        }
    }
}
