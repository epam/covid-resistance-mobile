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
using CoreBluetooth;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.TracingInformation;
using Foundation;
using Microsoft.Extensions.Logging;

namespace Covi.iOS.Features.Bluetooth.Advertising
{
    public class Advertiser
    {
        private readonly CBPeripheralManager _peripheralManager;
        private readonly ILogger _logger;
        private TracingInformation _tracingInformation;
        private bool _enabled;

        public Advertiser()
        {
            _peripheralManager = new CBPeripheralManager();
            _peripheralManager.StateUpdated += HandleStateUpdated;
            _peripheralManager.ReadRequestReceived += HandleReadRequestReceived;
            _logger = Covi.Logs.Logger.Get(this);
        }

        private void HandleReadRequestReceived(object sender, CBATTRequestEventArgs e)
        {
            Covi.Features.BluetoothTracing.TracingState.Instance.SetAdvertisingState(true);
        }

        private void HandleStateUpdated(object sender, EventArgs e)
        {
            if (_peripheralManager.State == CBPeripheralManagerState.PoweredOn)
            {
                if (_tracingInformation != null/* && !_enabled*/)
                {
                    _logger.LogDebug("Advertising restarting - " + _peripheralManager.State);
                    Restart();
                }
            }
            else
            {
                Stop();
                _logger.LogDebug("Advertising stopped - " + _peripheralManager.State);
            }

            _logger.LogDebug("Advertising state changed - " + _peripheralManager.State);
        }

        private void Restart()
        {
            Stop();
            Start(_tracingInformation);
        }

        public void Start(TracingInformation tracingInformation)
        {
            if (tracingInformation == null || _enabled)
            {
                return;
            }

            _tracingInformation = tracingInformation;
            _peripheralManager.RemoveAllServices();
            CBUUID uuidService = CBUUID.FromString(tracingInformation.ServiceId);
            CBUUID uuidCharacteristic = CBUUID.FromString(tracingInformation.CharacteristicId);
            var data = NSData.FromArray(PayloadFormatter.GetBytesToSend(new PackageData(tracingInformation.DeviceId)));
            var characteristic = new CBMutableCharacteristic(uuidCharacteristic, CBCharacteristicProperties.Read, data, CBAttributePermissions.Readable);
            var service = new CBMutableService(uuidService, true);
            service.Characteristics = new CBCharacteristic[] { characteristic };
            _peripheralManager.AddService(service);
            StartAdvertisingOptions advData = new StartAdvertisingOptions { ServicesUUID = new CBUUID[] { uuidService } };
            _peripheralManager.StartAdvertising(advData);
            TracingState.Instance.SetAdvertisingState(true);
            _enabled = true;
            _logger.LogDebug("Advertising starting. DeviceId: " + _tracingInformation.DeviceId);
        }

        public void Stop()
        {
            if (_peripheralManager == null)
            {
                // not started
                return;
            }

            _peripheralManager.StopAdvertising();
            Covi.Features.BluetoothTracing.TracingState.Instance.SetAdvertisingState(false);
            _enabled = false;
            _logger.LogDebug("Advertising - stopped");
        }
    }
}
