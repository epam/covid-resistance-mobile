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
    public class DispatcherService : IDispatcherService
    {
        public async Task InvokeAsync(Action action)
        {
            if (Xamarin.Essentials.MainThread.IsMainThread)
                action();
            else
                await Xamarin.Essentials.MainThread.InvokeOnMainThreadAsync(action).ConfigureAwait(false);
        }

        public async Task<T> InvokeAsync<T>(Func<T> function)
        {
            if (Xamarin.Essentials.MainThread.IsMainThread)
                return function();
            else
                return await Xamarin.Essentials.MainThread.InvokeOnMainThreadAsync(function).ConfigureAwait(false);
        }
    }
}
