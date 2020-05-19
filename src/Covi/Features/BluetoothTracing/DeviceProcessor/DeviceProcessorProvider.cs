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

namespace Covi.Features.BluetoothTracing.DeviceProcessor
{
    public static class DeviceProcessorProvider
    {
        private static readonly Lazy<IDeviceProcessor> DefaultInstance = new Lazy<IDeviceProcessor>(() =>
        {
            return new DeviceProcessor.Internal.DefaultDeviceProcessor();
        });

        private static readonly Func<IDeviceProcessor> DefaultInstanceProvider = () => DefaultInstance.Value;

        private static Func<IDeviceProcessor> _instanceProvider;

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static IDeviceProcessor GetInstance()
        {
            return (_instanceProvider ?? DefaultInstanceProvider).Invoke();
        }

        public static void Setup(Func<IDeviceProcessor> provider)
        {
            _instanceProvider = provider;
        }
    }
}
