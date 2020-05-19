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
using Covi.Services.Security.SecretsGenerator.Internal;

namespace Covi.Services.Security.SecretsGenerator
{
    public static class SecretsGenerator
    {

        private static readonly Lazy<ISecretsGenerator> DefaultInstance =
            new Lazy<ISecretsGenerator>(() => new DefaultSecretsGenerator());

        private static readonly Func<ISecretsGenerator> DefaultInstanceProvider = () => DefaultInstance.Value;

        private static Func<ISecretsGenerator> _instanceProvider;

        /// <summary>
        /// Gets the current secrets generator instance.
        /// </summary>
        public static ISecretsGenerator Instance
        {
            get { return (_instanceProvider ?? DefaultInstanceProvider).Invoke(); }
        }

        public static void Setup(Func<ISecretsGenerator> provider)
        {
            _instanceProvider = provider;
        }
    }
}
