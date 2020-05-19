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
using Covi.Features.AppSettings.Internal;

namespace Covi.Features.AppSettings
{
    public static class AppStateContainer
    {
        private static readonly Lazy<IAppStateContainer> DefaultInstance = new Lazy<IAppStateContainer>(() => new DefaultAppStateContainer());
        private static readonly Func<IAppStateContainer> DefaultInstanceProvider = () => DefaultInstance.Value;
        private static Func<IAppStateContainer> _instanceProvider;

        /// <summary>
        /// Gets the current instance of <see cref=" IAppStateContainer"/>.
        /// </summary>
        /// <returns><see cref="IAppStateContainer"/> instance if any.</returns>
        public static IAppStateContainer GetInstance()
        {
            return (_instanceProvider ?? DefaultInstanceProvider).Invoke();
        }

        public static void Setup(Func<IAppStateContainer> provider)
        {
            _instanceProvider = provider;
        }
    }
}
