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

namespace Covi.Features.AppSettings
{
    /// <summary>
    /// Provides an ability to store application specific preferences.
    /// </summary>
    public interface IAppPreferencesService
    {
        /// <summary>
        /// Gets a <c>string</c> value for the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key to retrieve.</param>
        /// <returns>Value if any.</returns>
        string GetValue(string key);

        /// <summary>
        /// Sets <c>string</c> <paramref name="value"/> for <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="value">Value to set.</param>
        void SetValue(string key, string value);

        bool GetBoolValue(string key);

        /// <summary>
        /// Sets <c>bool</c> <paramref name="value"/> for the <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key to set.</param>
        /// <param name="value">Value to set.</param>
        void SetBoolValue(string key, bool value);

        /// <summary>
        /// Gets or sets a value indicating whether the app needs to force update user info.
        /// </summary>
        bool IsAppNeedsForceRefreshUserStatus { get; set; }
    }
}
