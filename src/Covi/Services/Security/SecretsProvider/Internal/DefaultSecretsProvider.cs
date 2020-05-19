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

using System.Threading.Tasks;

namespace Covi.Services.Security.SecretsProvider.Internal
{
    /// <summary>
    /// <see cref="Xamarin.Essentials.SecureStorage"/> based <see cref="ISecretsProvider"/> implementation.
    /// </summary>
    public class DefaultSecretsProvider : ISecretsProvider
    {
        private static readonly string EncryptionKeyFieldName = Configuration.Constants.SecretsProviderConstants.EncryptionKeyFieldName;
        private static readonly string DeviceIdentifierFieldName = Configuration.Constants.SecretsProviderConstants.DeviceIdentifierFieldName;
        private static readonly string InitializedFlagFieldName = Configuration.Constants.SecretsProviderConstants.InitializedFlagFieldName;
        private static readonly string IsFirstRunFieldName = Configuration.Constants.SecretsProviderConstants.IsFirstRunFieldName;

        public async Task<string> GetEncryptionKeyAsync()
        {
            var key = await Xamarin.Essentials.SecureStorage.GetAsync(EncryptionKeyFieldName).ConfigureAwait(false);
            return key;
        }

        private async Task<string> GenerateEncryptionKey()
        {
            string key = string.Empty;
#if !DEBUG
                key = SecretsGenerator.SecretsGenerator.Instance.Generate();
#endif
            await SetEncryptionKeyAsync(key).ConfigureAwait(false);
            return key;
        }

        private async Task SetEncryptionKeyAsync(string value)
        {
            await Xamarin.Essentials.SecureStorage.SetAsync(EncryptionKeyFieldName, value)
                         .ConfigureAwait(false);
        }

        public async Task<string> GetDeviceIdentifierAsync()
        {
            var deviceId = await Xamarin.Essentials.SecureStorage.GetAsync(DeviceIdentifierFieldName).ConfigureAwait(false);
            return deviceId;
        }

        private async Task<string> GenerateDeviceIdentifier()
        {
            string deviceId = SecretsGenerator.SecretsGenerator.Instance.GenerateDeviceId();
            await SetDeviceIdentifierAsync(deviceId).ConfigureAwait(false);
            return deviceId;
        }

        private async Task SetDeviceIdentifierAsync(string value)
        {
            await Xamarin.Essentials.SecureStorage.SetAsync(DeviceIdentifierFieldName, value)
                         .ConfigureAwait(false);
        }

        public async Task InitializeAsync()
        {
            await GenerateDeviceIdentifier().ConfigureAwait(false);
            await GenerateEncryptionKey().ConfigureAwait(false);
            Xamarin.Essentials.Preferences.Set(InitializedFlagFieldName, true);
            Xamarin.Essentials.Preferences.Set(IsFirstRunFieldName, false);
        }

        public async Task InitializeAsync(string deviceId)
        {
            await SetDeviceIdentifierAsync(deviceId).ConfigureAwait(false);
            await GenerateEncryptionKey().ConfigureAwait(false);
            Xamarin.Essentials.Preferences.Set(InitializedFlagFieldName, true);
            Xamarin.Essentials.Preferences.Set(IsFirstRunFieldName, false);
        }

        public bool GetIsInitialized()
        {
            var initialized = Xamarin.Essentials.Preferences.Get(InitializedFlagFieldName, false);
            // If it is a first launch ever, but we have some data in keychain - need to remove it
            var isFirstLaunch = Xamarin.Essentials.Preferences.Get(IsFirstRunFieldName, true);
            if (initialized && isFirstLaunch)
            {
                AsyncHelpers.RunSync(CleanupAsync);
                initialized = false;
            }

            return initialized;
        }

        public Task CleanupAsync()
        {
            Xamarin.Essentials.SecureStorage.Remove(InitializedFlagFieldName);
            Xamarin.Essentials.SecureStorage.Remove(DeviceIdentifierFieldName);
            Xamarin.Essentials.SecureStorage.Remove(EncryptionKeyFieldName);
            return Task.CompletedTask;
        }
    }
}
