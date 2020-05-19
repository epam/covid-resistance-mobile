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

namespace Covi.Services.Security.SecureVault
{
    public class SecureVault : ISecureVault
    {
        public async Task AddAsync(string key, string data)
        {
            await Xamarin.Essentials.SecureStorage.SetAsync(key, data).ConfigureAwait(false);
        }

        public async Task<string> GetAsync(string key)
        {
            return await Xamarin.Essentials.SecureStorage.GetAsync(key).ConfigureAwait(false);
        }

        public Task RemoveAsync(string key)
        {
            Xamarin.Essentials.SecureStorage.Remove(key);
            return Task.CompletedTask;
        }
    }
}
