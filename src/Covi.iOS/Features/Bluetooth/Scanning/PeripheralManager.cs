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
using System.Linq;
using CoreBluetooth;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.DeviceManager;
using Covi.Features.BluetoothTracing.TracingInformation;
using Foundation;
using Microsoft.Extensions.Logging;

namespace Covi.iOS.Features.Bluetooth.Scanning
{
    public class PeripheralManager
    {
        private readonly CBCentralManager _centralManager;
        private readonly TracingInformation _tracingInformation;
        private ILogger<PeripheralManager> _logger;
        private IDeviceManager _manager;

        public PeripheralManager(CBCentralManager centralManager, TracingInformation tracingInformation)
        {
            _centralManager = centralManager;
            _tracingInformation = tracingInformation;
            _logger = Covi.Logs.Logger.Get(this);
            _manager = DeviceManagerProvider.GetInstance();
        }

        public void HandleDiscoveredPeripheral(CBDiscoveredPeripheralEventArgs evnt)
        {
            var descriptor = evnt.Peripheral.ToDeviceDescriptor();

            _manager.HandleDeviceDiscovered(descriptor, (d) =>
            {
                _centralManager.ConnectPeripheral(evnt.Peripheral);
            });
        }

        public void HandleConnectedPeripheral(CBPeripheralEventArgs evnt)
        {
            var descriptor = evnt.Peripheral.ToDeviceDescriptor();

            _manager.HandleDeviceConnected(descriptor, (d) =>
            {
                evnt.Peripheral.Delegate = new PeripheralDelegate(_centralManager, _manager, _tracingInformation);
                evnt.Peripheral.DiscoverServices();
            });
        }

        public void HandleDisconnectedPeripheral(CBPeripheralErrorEventArgs evnt)
        {
            var descriptor = evnt.Peripheral.ToDeviceDescriptor();

            _manager.HandleDeviceDisconnected(descriptor, null);
        }

        public void HandleFailedToConnectPeripheral(CBPeripheralErrorEventArgs evnt)
        {
            var descriptor = evnt.Peripheral.ToDeviceDescriptor();

            _manager.HandleDeviceFailedToConnect(descriptor, null);
        }

        public class PeripheralDelegate : CBPeripheralDelegate
        {
            private readonly CBCentralManager _centralManager;
            private readonly IDeviceManager _deviceManager;
            private readonly TracingInformation _tracingInformation;
            private ILogger _logger;

            public PeripheralDelegate(CBCentralManager centralManager, IDeviceManager deviceManager, TracingInformation tracingInformation)
            {
                _centralManager = centralManager;
                _deviceManager = deviceManager;
                _tracingInformation = tracingInformation;
                _logger = Covi.Logs.Logger.Get(this);
            }

            public override void DiscoveredService(CBPeripheral peripheral, NSError error)
            {
                var descriptor = peripheral.ToDeviceDescriptor();

                if (error != null)
                {
                    _deviceManager.HandleDeviceCommunicationDiscoveryServiceError(descriptor, error.LocalizedFailureReason, (d) =>
                    {
                        _centralManager.CancelPeripheralConnection(peripheral);
                    });
                    return;
                }

                CBUUID uuidCharacteristic = CBUUID.FromString(_tracingInformation.CharacteristicId);
                CBUUID uuidService = CBUUID.FromString(_tracingInformation.ServiceId);
                var service = peripheral.Services.FirstOrDefault(x => x.UUID == uuidService);

                if (service != null)
                {
                    _deviceManager.HandleDeviceCommunicationDiscoveredService(descriptor, (d) =>
                    {
                        peripheral.DiscoverCharacteristics(new[] { uuidCharacteristic }, service);
                    });
                }
                else
                {
                    _deviceManager.HandleIncorrectDevice(descriptor);
                    _centralManager.CancelPeripheralConnection(peripheral);
                }
            }

            public override void DiscoveredCharacteristic(CBPeripheral peripheral, CBService service, NSError error)
            {
                var descriptor = peripheral.ToDeviceDescriptor();

                if (error != null)
                {
                    _deviceManager.HandleDeviceCommunicationDiscoveredCharacteristicError(descriptor, error.LocalizedFailureReason, (d) =>
                    {
                        _centralManager.CancelPeripheralConnection(peripheral);
                    });
                    return;
                }

                var characteristic = service.Characteristics?.FirstOrDefault();

                if (characteristic != null)
                {
                    _deviceManager.HandleDeviceCommunicationDiscoveredCharacteristic(descriptor, (d) =>
                    {
                        peripheral.ReadValue(characteristic);
                    });
                }
                else
                {
                    _deviceManager.HandleIncorrectDevice(descriptor);
                    _centralManager.CancelPeripheralConnection(peripheral);
                }
            }

            public override void UpdatedCharacterteristicValue(CBPeripheral peripheral, CBCharacteristic characteristic, NSError error)
            {
                var descriptor = peripheral.ToDeviceDescriptor();

                if (error != null)
                {
                    _deviceManager.HandleDeviceCommunicationDiscoveredCharacteristicReadError(descriptor, error.LocalizedFailureReason, (d) =>
                    {
                        _centralManager.CancelPeripheralConnection(peripheral);
                    });
                    return;
                }

                try
                {
                    var payload = characteristic.Value.ToArray();

                    _deviceManager.HandleDeviceCommunicationDiscoveredCharacteristicRead(descriptor, payload);

                    // Disconnect peripheral
                    _centralManager.CancelPeripheralConnection(peripheral);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                            $"Scanner - Peripheral - Service - Processing failed. id: {peripheral.Identifier}, characteristicId: {characteristic.UUID}. Reason {error.LocalizedFailureReason}.");
                }
            }
        }
    }

    public static class PeripheralExtensions
    {
        public static DeviceDescriptor ToDeviceDescriptor(this CBPeripheral peripheral)
        {
            var result = new DeviceDescriptor(peripheral.Identifier.AsString())
            {
                Name = peripheral.Name,
                DiscoveryTime = DateTime.UtcNow,
                // TODO implement rssi reading logic
                //RSSI = peripheral.RSSI.Int32Value,
                Context = peripheral,
            };
            return result;
        }
    }
}
