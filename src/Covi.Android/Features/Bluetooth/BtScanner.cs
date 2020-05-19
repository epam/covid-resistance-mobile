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
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.OS;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.DeviceManager;
using Covi.Features.BluetoothTracing.TracingInformation;
using Microsoft.Extensions.Logging;

namespace Covi.Droid.Features.Bluetooth
{
    public class BtScanner
    {
        private BluetoothLeScanner _leScanner;
        private ScanCallback _scanCallback;
        private TracingInformation _tracingInformation;
        private ILogger _logger;
        private IDeviceManager _manager;

        public bool Initialized { get; private set; }

        public void Init()
        {
            BluetoothAdapter btAdapter = BluetoothAdapter.DefaultAdapter;
            _leScanner = btAdapter?.BluetoothLeScanner;
            Initialized = _leScanner != null;
            _manager = DeviceManagerProvider.GetInstance();
            _logger = Covi.Logs.Logger.Get(this);
        }

        public void StartScanning(Context context, TracingInformation tracingInformation)
        {
            if (!Initialized)
            {
                _logger.LogError("Scanner - Starting failed - not initialized.");
                return;
            }

            try
            {
                _tracingInformation = tracingInformation;

                _logger.LogDebug($"Scanner - Starting for service {_tracingInformation.ServiceId}.");

                if (_scanCallback == null)
                {
                    _scanCallback = new BtScanCallback(context, _manager, tracingInformation);
                }
                System.Threading.Tasks.Task.Run(() =>
                {
                    _leScanner.StartScan(BuildScanFilters(_tracingInformation), BuildScanSettings(), _scanCallback);
                }).FireAndForget();
                Covi.Features.BluetoothTracing.TracingState.Instance.SetScanningState(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scanner - Starting failed.");
            }
        }

        public void StopScanning()
        {
            if (!Initialized)
            {
                return;
            }

            _leScanner.StopScan(_scanCallback);
            _scanCallback?.Dispose();
            _scanCallback = null;
            Covi.Features.BluetoothTracing.TracingState.Instance.SetScanningState(false);
        }

        private ScanSettings BuildScanSettings()
        {
            var builder = new ScanSettings.Builder();
            builder.SetScanMode(Android.Bluetooth.LE.ScanMode.LowPower);
            return builder.Build();
        }

        private IList<ScanFilter> BuildScanFilters(TracingInformation tracingInformation)
        {
            var scanFilters = new List<ScanFilter>();
            var builder = new ScanFilter.Builder();
            var serviceUuid = ParcelUuid.FromString(tracingInformation.ServiceId);
            builder.SetServiceUuid(serviceUuid);
            scanFilters.Add(builder.Build());
            return scanFilters;
        }

        private class BtScanCallback : ScanCallback
        {
            private readonly Context _context;
            private readonly IDeviceManager _deviceManager;
            private readonly TracingInformation _tracingInformation;
            private readonly ILogger _logger;

            public BtScanCallback(Context context, IDeviceManager deviceManager, TracingInformation tracingInformation)
            {
                this._context = context;
                _deviceManager = deviceManager;
                _tracingInformation = tracingInformation;
                _logger = Covi.Logs.Logger.Get(this);
            }

            public override void OnBatchScanResults(IList<ScanResult> results)
            {
                base.OnBatchScanResults(results);
                foreach (ScanResult result in results)
                {
                    if (result != null)
                    {
                        ProcessResult(result);
                    }
                }
            }

            public override void OnScanResult(ScanCallbackType callbackType, ScanResult result)
            {
                base.OnScanResult(callbackType, result);
                if (result != null)
                {
                    ProcessResult(result);
                }
            }

            public override void OnScanFailed(ScanFailure errorCode)
            {
                _logger.LogError("Scanner - scan failed. Error: " + errorCode.ToString());
                base.OnScanFailed(errorCode);
            }

            private void ProcessResult(ScanResult result)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    BtDevice btDevice = new BtDevice(result.Device.Address, result.TimestampNanos);
                    var descriptor = result.ToDeviceDescriptor();
                    _deviceManager.HandleDeviceDiscovered(descriptor, (d) =>
                    {
                        result.Device.ConnectGatt(
                                   _context,
                                   false,
                                   new BtGattConnectCallback(btDevice, descriptor, _deviceManager, _tracingInformation));
                    });
                }).FireAndForget();
            }
        }

        class BtGattConnectCallback : BluetoothGattCallback
        {
            private readonly BtDevice _btDevice;
            private readonly DeviceDescriptor _descriptor;
            private readonly IDeviceManager _deviceManager;
            private readonly TracingInformation _tracingInformation;

            public BtGattConnectCallback(BtDevice btDevice, DeviceDescriptor descriptor, IDeviceManager deviceManager, TracingInformation tracingInformation)
            {
                _btDevice = btDevice;
                _descriptor = descriptor;
                _deviceManager = deviceManager;
                _tracingInformation = tracingInformation;
            }

            public override void OnConnectionStateChange(BluetoothGatt gatt, GattStatus status, ProfileState newState)
            {
                base.OnConnectionStateChange(gatt, status, newState);
                System.Threading.Tasks.Task.Run(() =>
                {
                    if (newState == ProfileState.Connected)
                    {
                        if (gatt != null)
                        {
                            _deviceManager.HandleDeviceConnected(_descriptor, (d) =>
                            {
                                gatt.DiscoverServices();
                            });
                        }
                    }
                    else if (newState == ProfileState.Disconnected)
                    {
                        _deviceManager.HandleDeviceDisconnected(_descriptor);
                    }
                }).FireAndForget();
            }

            public override void OnServicesDiscovered(BluetoothGatt gatt, GattStatus status)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    base.OnServicesDiscovered(gatt, status);
                    if (gatt != null)
                    {
                        try
                        {
                            var serviceUuid = ParcelUuid.FromString(_tracingInformation.ServiceId);
                            BluetoothGattService service = gatt.GetService(serviceUuid.Uuid);

                            if (service != null)
                            {
                                _deviceManager.HandleDeviceCommunicationDiscoveredService(_descriptor, (d) =>
                                {
                                    var characteristicUuid = ParcelUuid.FromString(_tracingInformation.CharacteristicId);
                                    BluetoothGattCharacteristic characteristic = service.GetCharacteristic(characteristicUuid.Uuid);

                                    if (characteristic != null)
                                    {
                                        _deviceManager.HandleDeviceCommunicationDiscoveredCharacteristic(_descriptor, (d) =>
                                        {
                                            gatt.ReadCharacteristic(characteristic);
                                        });
                                    }
                                    else
                                    {
                                        _deviceManager.HandleIncorrectDevice(_descriptor, (d) =>
                                     {
                                         gatt.Disconnect();
                                     });
                                    }
                                });
                            }
                            else
                            {
                                _deviceManager.HandleIncorrectDevice(_descriptor, (d) =>
                                {
                                    gatt.Disconnect();
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            _deviceManager.HandleDeviceCommunicationDiscoveryServiceError(_descriptor, ex.Message, (d) =>
                            {
                                gatt.Disconnect();
                            });
                        }
                    }
                }).FireAndForget();
            }

            public override void OnCharacteristicRead(BluetoothGatt gatt, BluetoothGattCharacteristic characteristic, GattStatus status)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    try
                    {
                        base.OnCharacteristicRead(gatt, characteristic, status);
                        var payload = characteristic.GetValue();
                        _deviceManager.HandleDeviceCommunicationDiscoveredCharacteristicRead(_descriptor, payload);
                        gatt.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        _deviceManager.HandleDeviceCommunicationDiscoveredCharacteristicReadError(_descriptor, ex.Message);
                    }
                }).FireAndForget();
            }
        }
    }

    public static class ScanResultExtensions
    {
        public static DeviceDescriptor ToDeviceDescriptor(this ScanResult scanResult)
        {
            var result = new DeviceDescriptor(scanResult.Device.Address)
            {
                RSSI = scanResult.Rssi,
                DiscoveryTime = DateTime.UtcNow,
                Context = scanResult
            };
            return result;
        }
    }
}
