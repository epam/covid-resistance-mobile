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

namespace Covi.Services.Dispatcher
{
    /// <summary>
    /// Provides capabilities to invoke action on the main thread.
    /// </summary>
    public interface IDispatcherService
    {
        /// <summary>
        /// Invokes provided <paramref name="action"/> on the main thread.
        /// </summary>
        /// <param name="action">Action to invoke.</param>
        /// <returns>Task to await.</returns>
        Task InvokeAsync(Action action);

        /// <summary>
        /// Invokes provided <paramref name="function"/> on the main thread.
        /// </summary>
        /// <typeparam name="T">Type of the result.</typeparam>
        /// <param name="function">Function to invoke.</param>
        /// <returns>Task to await for with a result of type <typeparamref name="T"/>.</returns>
        Task<T> InvokeAsync<T>(Func<T> function);
    }
}
