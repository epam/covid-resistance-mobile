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
using Microsoft.Extensions.Logging;

namespace Covi.Features.AppSettings.Internal
{
    /// <summary>
    /// Default implementation of <see cref="IAppPreferencesService"/> based on <see cref="Xamarin.Essentials.Preferences"/>.
    /// </summary>
    public class DefaultAppPreferencesService : IAppPreferencesService
    {
        private const string ForceStatusRefreshKey = "ForceStatusRefresh";

        private readonly ILogger _logger;

        public DefaultAppPreferencesService()
        {
            _logger = Covi.Logs.Logger.Get(this);
        }

        public string GetValue(string key)
        {
            var result = string.Empty;

            try
            {
                result = Xamarin.Essentials.Preferences.Get(key, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get value of the app preference {key} from preferences.");
            }

            return result;
        }

        public bool GetBoolValue(string key)
        {
            var result = false;

            try
            {
                result = Xamarin.Essentials.Preferences.Get(key, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get value of the app preference {key} from preferences.");
            }

            return result;
        }

        public void SetValue(string key, string value)
        {
            try
            {
                Xamarin.Essentials.Preferences.Set(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to set value of the app preference {key} to preferences.");
            }
        }

        public void SetBoolValue(string key, bool value)
        {
            try
            {
                Xamarin.Essentials.Preferences.Set(key, value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to set value of the app preference {key} to preferences.");
            }
        }

        public bool IsAppNeedsForceRefreshUserStatus
        {
            get => GetBoolValue(ForceStatusRefreshKey);
            set
            {
                SetBoolValue(ForceStatusRefreshKey, value);
                try
                {
                    if (value && AppStateContainer.GetInstance().IsAppActive)
                    {
                        Xamarin.Forms.Application.Current.SendOnAppLinkRequestReceived(DeepLinks.DeepLinks.UserStatusUpdateLink);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send update deeplink.");
                }
            }
        }
    }
}
