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
    /// <summary>
    /// Provides an ability to securely store small amount of data.
    /// </summary>
    public interface ISecureVault
    {
        /// <summary>
        /// Add <paramref name="data"/> for <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key to associate with.</param>
        /// <param name="data">Data to store.</param>
        /// <returns>Task to await.</returns>
        Task AddAsync(string key, string data);

        /// <summary>
        /// Get data associated with <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key to extract.</param>
        /// <returns>Data, if any.</returns>
        Task<string> GetAsync(string key);

        /// <summary>
        /// Removes the data associated with <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key to delete.</param>
        /// <returns>Task to await.</returns>
        Task RemoveAsync(string key);
    }
}
