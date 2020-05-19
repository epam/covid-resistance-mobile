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
using System.Threading.Tasks;

namespace Covi.Services
{
    /// <summary>
    /// Container to store information of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of the data.</typeparam>
    public interface IReactiveContainer<T>
    {
        /// <summary>
        /// Stream of data changes.
        /// </summary>
        IObservable<T> Changes { get; }

        /// <summary>
        /// Gets tje data.
        /// </summary>
        /// <returns>Returns type of <typeparamref name="T"/> if present, otherwise null.</returns>
        Task<T> GetAsync();

        /// <summary>
        /// Sets <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Data to be set.</param>
        /// <returns>Task to await.</returns>
        Task SetAsync(T data);
    }
}
