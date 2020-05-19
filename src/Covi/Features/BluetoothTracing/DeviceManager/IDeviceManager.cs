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
using System.Collections.Generic;
using System.Threading;

namespace Covi.Features.BluetoothTracing.DeviceManager
{
    /// <summary>
    /// This manage is responsible for controlling the device discovery and communication flow.
    /// </summary>
    public interface IDeviceManager
    {
        void HandleDeviceConnected(DeviceDescriptor deviceDescriptor, Action<DeviceDescriptor> onNext = null);

        void HandleDeviceDisconnected(DeviceDescriptor deviceDescriptor, Action<DeviceDescriptor> onNext = null);

        void HandleDeviceDiscovered(DeviceDescriptor deviceDescriptor, Action<DeviceDescriptor> onNext = null);

        void HandleDeviceFailedToConnect(DeviceDescriptor descriptor, Action<DeviceDescriptor> onNext = null);

        void HandleDeviceCommunicationDiscoveredCharacteristic(DeviceDescriptor descriptor, Action<DeviceDescriptor> onNext = null);

        void HandleDeviceCommunicationDiscoveredCharacteristicError(DeviceDescriptor descriptor, string error, Action<DeviceDescriptor> onNext = null);

        void HandleDeviceCommunicationDiscoveredCharacteristicRead(DeviceDescriptor descriptor, byte[] payload, Action<DeviceDescriptor> onNext = null);

        void HandleDeviceCommunicationDiscoveredCharacteristicReadError(DeviceDescriptor descriptor, string error, Action<DeviceDescriptor> onNext = null);

        void HandleDeviceCommunicationDiscoveredService(DeviceDescriptor descriptor, Action<DeviceDescriptor> onNext = null);

        void HandleDeviceCommunicationDiscoveryServiceError(DeviceDescriptor descriptor, string error, Action<DeviceDescriptor> onNext = null);

        void HandleIncorrectDevice(DeviceDescriptor descriptor, Action<DeviceDescriptor> onNext = null);
    }

    public class DeviceDescriptor : IEqualityComparer<DeviceDescriptor>
    {
        public DeviceDescriptor(string identifier)
        {
            Identifier = identifier;
        }

        public DateTime DiscoveryTime { get; set; }

        public int RSSI { get; set; }

        public string Identifier { get; }

        public string Name { get; set; }

        public bool Processed { get; set; }

        public bool Processing { get; set; }

        public object ProcessingLock { get; } = new object();

        public bool Ignore { get; internal set; }

        public object Context { get; set; }

        public bool Equals(DeviceDescriptor x, DeviceDescriptor y)
        {
            return x != null && string.Equals(x?.Identifier, y?.Identifier, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(DeviceDescriptor obj)
        {
            return obj?.Identifier?.GetHashCode() ?? 0;
        }
    }
}
