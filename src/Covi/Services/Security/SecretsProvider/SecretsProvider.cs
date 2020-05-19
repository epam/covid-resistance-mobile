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
using Covi.Services.Security.SecretsProvider.Internal;

namespace Covi.Services.Security.SecretsProvider
{
    /// <summary>
    /// Provides an access to application secrets.
    /// </summary>
    public static class SecretsProvider
    {
        private static readonly Lazy<ISecretsProvider> DefaultInstance =
            new Lazy<ISecretsProvider>(() => new DefaultSecretsProvider());

        private static readonly Func<ISecretsProvider> DefaultInstanceProvider = () => DefaultInstance.Value;

        private static Func<ISecretsProvider> _instanceProvider;

        /// <summary>
        /// Gets the current secrets provider instance.
        /// </summary>
        public static ISecretsProvider Instance
        {
            get { return (_instanceProvider ?? DefaultInstanceProvider).Invoke(); }
        }

        public static void Setup(Func<ISecretsProvider> provider)
        {
            _instanceProvider = provider;
        }
    }
}
