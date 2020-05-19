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

namespace Covi.Services.Storage
{
    /// <summary>
    /// Provides an ability to persist objects.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Stores <paramref name="data"/> under <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">Payload type.</typeparam>
        /// <param name="key">Key.</param>
        /// <param name="data">Data.</param>
        /// <returns>Task to await.</returns>
        Task AddAsync<T>(string key, T data)
            where T : class;

        /// <summary>
        /// Gets data of type <typeparamref name="T"/> stored under <paramref name="key"/>.
        /// </summary>
        /// <typeparam name="T">Payload type.</typeparam>
        /// <param name="key">Key.</param>
        /// <returns>Result of type <typeparamref name="T"/>, otherwise default(T).</returns>
        Task<T> GetAsync<T>(string key)
            where T : class;

        /// <summary>
        /// Clears all data associated with <paramref name="key"/>.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <returns>Task to await.</returns>
        Task ClearAsync(string key);

        /// <summary>
        /// Suspends the database instance.
        /// </summary>
        void Suspend();
    }
}
