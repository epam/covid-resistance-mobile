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
using Covi.Features.BluetoothTracing.TracesStorage.LiteDbStorage;
using Covi.Services.Security.SecretsProvider;
using Covi.Services.Storage.LiteDbStorage;

namespace Covi.Features.BluetoothTracing.TracesStorage
{
    /// <summary>
    /// Provides an access to an application tracing storage.
    /// </summary>
    public static class TracesStorage
    {
        private static ITracesStorage _instance;

        private static readonly Lazy<ITracesStorage> DefaultInstance =
            new Lazy<ITracesStorage>(() =>
                                     {
                                         var key = AsyncHelpers.RunSync(() => SecretsProvider.Instance.GetEncryptionKeyAsync());
                                         var options = new StorageOptions()
                                         {
                                             EncryptionKey = key
                                         };
                                         return new LiteDbTracesStorage(options);
                                     });

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static ITracesStorage Instance
        {
            get { return _instance ??= DefaultInstance.Value; }
        }

        public static void Setup(ITracesStorage provider)
        {
            _instance = provider;
        }
    }
}
