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
using System.Threading;
using System.Threading.Tasks;
using Covi.Features.BluetoothTracing.DeviceProcessor;
using Microsoft.Extensions.Logging;

namespace Covi.Features.BluetoothTracing.DeviceManager.Internal
{
    public class DefaultDeviceManager : IDeviceManager
    {
        private System.Collections.Concurrent.ConcurrentDictionary<string, DeviceDescriptor> _discoveredDevices = new System.Collections.Concurrent.ConcurrentDictionary<string, DeviceDescriptor>();
        private ILogger _logger;
        private DateTime _lastCleanupTime;
        private object _cleanupLock = new object();
        private object _discoveryLock = new object();

        public DefaultDeviceManager()
        {
            _logger = Covi.Logs.Logger.Get(this);
        }

        private void InvokeCleanupIfNeeded()
        {
            if (!((DateTime.UtcNow - _lastCleanupTime) <= Configuration.Constants.BluetoothConstants.DeviceThrottleTime))
            {
                return;
            }

            Task.Run(() =>
            {
                if (!Monitor.TryEnter(_cleanupLock))
                {
                    return;
                }

                try
                {
                    if (!((DateTime.UtcNow - _lastCleanupTime) <= Configuration.Constants.BluetoothConstants.DeviceThrottleTime))
                    {
                        return;
                    }

                    _logger.LogDebug($"DeviceManager - Device cleanup started.");

                    var devices = _discoveredDevices.Values.ToList();
                    foreach (var device in devices)
                    {
                        if (_discoveredDevices.TryGetValue(device.Identifier, out var descriptor))
                        {
                            if (DateTime.UtcNow - descriptor.DiscoveryTime >= Configuration.Constants.BluetoothConstants.DeviceThrottleTime)
                            {
                                _discoveredDevices.TryRemove(device.Identifier, out var _);
                            }
                        }
                    }

                    _lastCleanupTime = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"DeviceManager - Device cleanup failed.");
                }
                finally
                {
                    Monitor.Exit(_cleanupLock);
                }
            }).FireAndForget();
        }

        public void HandleDeviceDiscovered(DeviceDescriptor deviceDescriptor, Action<DeviceDescriptor> onNext = null)
        {
            lock (_discoveryLock)
            {
                try
                {
                    DeviceDescriptor descriptor = deviceDescriptor;
                    deviceDescriptor.Processing = true;
                    var existingDevice = _discoveredDevices.GetOrAdd(deviceDescriptor.Identifier, deviceDescriptor);
                    if (existingDevice != null && existingDevice != deviceDescriptor)
                    {
                        // If some other thread already processing - then cancel
                        if (!Monitor.TryEnter(deviceDescriptor.ProcessingLock))
                        {
                            return;
                        }

                        try
                        {
                            // TODO: log contact time here for further analysis of exposure time and signal strength
                            if (!existingDevice.Processing && !existingDevice.Ignore)
                            {
                                // Avoid processing already processed devices
                                if (existingDevice.Processed)
                                {
                                    if (DateTime.UtcNow - existingDevice.DiscoveryTime <=
                                        Configuration.Constants.BluetoothConstants.DeviceThrottleTime)
                                    {
                                        return;
                                    }
                                }

                                //Update context information
                                existingDevice.Context = deviceDescriptor.Context;
                                existingDevice.DiscoveryTime = deviceDescriptor.DiscoveryTime;
                                existingDevice.RSSI = deviceDescriptor.RSSI;
                                existingDevice.Processing = true;
                                existingDevice.Processed = false;
                                descriptor = existingDevice;
                            }
                            else
                            {
                                // device already being processed
                                return;
                            }
                        }
                        finally
                        {
                            Monitor.Exit(deviceDescriptor.ProcessingLock);
                        }
                    }

                    _logger.LogDebug($"DeviceManager - Device Discovered - Connecting. id: {descriptor.Identifier}.");

                    onNext?.Invoke(descriptor);

                    InvokeCleanupIfNeeded();
                }
                catch (Exception e)
                {
                    _logger.LogError(
                        e, $"DeviceManager - Device Discovered - Error. id: {deviceDescriptor.Identifier}.");
                }
            }
        }

        public void HandleDeviceConnected(DeviceDescriptor deviceDescriptor, Action<DeviceDescriptor> onNext = null)
        {
            if (_discoveredDevices.TryGetValue(deviceDescriptor.Identifier, out var existingDevice))
            {
                try
                {
                    // Update context
                    existingDevice.Context = deviceDescriptor.Context;
                    _logger.LogDebug(
                        $"DeviceManager - Device connected - Starting communication. id: {deviceDescriptor.Identifier}.");
                    onNext?.Invoke(existingDevice);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        $"DeviceManager - Device connected - communication failed. id: {deviceDescriptor.Identifier}.");
                }
            }
        }

        public void HandleDeviceDisconnected(DeviceDescriptor deviceDescriptor, Action<DeviceDescriptor> onNext = null)
        {
            if (_discoveredDevices.TryGetValue(deviceDescriptor.Identifier, out var existingDevice))
            {
                try
                {
                    Monitor.Enter(deviceDescriptor.ProcessingLock);
                    try
                    {
                        _logger.LogDebug($"DeviceManager - Device disconnected. id: {deviceDescriptor.Identifier}.");
                        // Clean up context as it may reference some native objects
                        existingDevice.Context = null;

                        existingDevice.Processing = false;
                    }
                    finally
                    {
                        Monitor.Exit(deviceDescriptor.ProcessingLock);
                    }

                    onNext?.Invoke(deviceDescriptor);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"DeviceManager - Device disconnected - failed. id: {existingDevice.Identifier}.");
                }
            }
        }

        public void HandleDeviceFailedToConnect(DeviceDescriptor descriptor, Action<DeviceDescriptor> onNext = null)
        {
            if (_discoveredDevices.TryGetValue(descriptor.Identifier, out var deviceDescriptor))
            {
                try
                {
                    Monitor.Enter(deviceDescriptor.ProcessingLock);
                    try
                    {
                        _logger.LogDebug(
                        $"DeviceManager - Device connection failed. id: {deviceDescriptor.Identifier}.");
                        deviceDescriptor.Processing = false;
                        // Clean up context as it may reference some native objects
                        deviceDescriptor.Context = null;
                        HandleIncorrectDevice(descriptor);
                    }
                    finally
                    {
                        Monitor.Exit(deviceDescriptor.ProcessingLock);
                    }

                    onNext?.Invoke(descriptor);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"DeviceManager - Device connection failed - failed. id: {deviceDescriptor.Identifier}.");
                }
            }
        }

        public void HandleDeviceCommunicationDiscoveredService(DeviceDescriptor descriptor, Action<DeviceDescriptor> onNext = null)
        {
            if (_discoveredDevices.TryGetValue(descriptor.Identifier, out var deviceDescriptor))
            {
                try
                {
                    _logger.LogDebug($"DeviceManager - Service discovery completed - processing characteristics. id: {deviceDescriptor.Identifier}.");
                    onNext?.Invoke(descriptor);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"DeviceManager - Service discovery completed - processing characteristics - failed. id: {deviceDescriptor.Identifier}.");
                }
            }
        }

        public void HandleDeviceCommunicationDiscoveryServiceError(DeviceDescriptor descriptor, string error, Action<DeviceDescriptor> onNext = null)
        {
            try
            {
                _logger.LogDebug(
                    $"DeviceManager - Service discovery failed. id: {descriptor.Identifier}. Reason: {error}.");
                HandleIncorrectDevice(descriptor);

                onNext?.Invoke(descriptor);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    $"DeviceManager - Service discovery failed - processing failed. id: {descriptor.Identifier}.");
            }
        }

        public void HandleIncorrectDevice(DeviceDescriptor descriptor, Action<DeviceDescriptor> onNext = null)
        {
            if (_discoveredDevices.TryGetValue(descriptor.Identifier, out var deviceDescriptor))
            {
                try
                {
                    _logger.LogDebug(
                        $"DeviceManager - Service discovery - incorrect device. id: {descriptor.Identifier}.");
                    try
                    {
                        Monitor.Enter(deviceDescriptor.ProcessingLock);
                        deviceDescriptor.Ignore = true;
                    }
                    finally
                    {
                        Monitor.Exit(deviceDescriptor.ProcessingLock);
                    }

                    // Mark device to be ignored as being incorrect (other protocol)
                    onNext?.Invoke(descriptor);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        $"DeviceManager - Service discovery - incorrect device - processing failed. id: {descriptor.Identifier}.");
                }
            }
        }

        public void HandleDeviceCommunicationDiscoveredCharacteristic(DeviceDescriptor descriptor, Action<DeviceDescriptor> onNext = null)
        {
            try
            {
                _logger.LogDebug($"DeviceManager - Characteristic discovery completed - processing value. id: {descriptor.Identifier}.");
                onNext?.Invoke(descriptor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeviceManager - Characteristic discovery completed - failed. id: {descriptor.Identifier}.");
            }
        }

        public void HandleDeviceCommunicationDiscoveredCharacteristicError(DeviceDescriptor descriptor, string error, Action<DeviceDescriptor> onNext = null)
        {
            try
            {
                _logger.LogDebug($"DeviceManager - Characteristic discovery failed. id: {descriptor.Identifier}. Reason: {error}.");
                HandleIncorrectDevice(descriptor);
                onNext?.Invoke(descriptor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeviceManager - Characteristic discovery failed - processing failed. id: {descriptor.Identifier}.");
            }
        }

        public void HandleDeviceCommunicationDiscoveredCharacteristicRead(DeviceDescriptor descriptor, byte[] payload, Action<DeviceDescriptor> onNext = null)
        {
            try
            {
                if (_discoveredDevices.TryGetValue(descriptor.Identifier, out var deviceDescriptor))
                {
                    try
                    {
                        if (PayloadFormatter.TryGetValue(payload, out var packageData))
                        {
                            _logger.LogDebug(
                                $"DeviceManager - Characteristic read - processing. id: {descriptor.Identifier}.");
                            deviceDescriptor.Context = descriptor.Context;
                            var contact = new ContactDescriptor(packageData.DeviceId, DateTime.UtcNow);

                            DeviceProcessorProvider.GetInstance().Process(contact);
                            Monitor.Enter(deviceDescriptor.ProcessingLock);
                            try
                            {
                                deviceDescriptor.Processed = true;
                            }
                            finally
                            {
                                Monitor.Exit(deviceDescriptor.ProcessingLock);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"DeviceManager - Characteristic read - Processing failed. id: {deviceDescriptor.Identifier}.");
                    }

                    // Disconnect peripheral
                    onNext?.Invoke(deviceDescriptor);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                        $"DeviceManager - Characteristic read - Processing failed. id: {descriptor.Identifier}. Reason: {ex.Message}.");
            }
        }

        public void HandleDeviceCommunicationDiscoveredCharacteristicReadError(DeviceDescriptor descriptor, string error, Action<DeviceDescriptor> onNext = null)
        {
            try
            {
                _logger.LogDebug($"DeviceManager - Characteristic read failed. id: {descriptor.Identifier}. Reason: {error}.");
                HandleIncorrectDevice(descriptor);
                onNext?.Invoke(descriptor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeviceManager - Characteristic read failed - processing failed. id: {descriptor.Identifier}.");
            }
        }
    }
}
