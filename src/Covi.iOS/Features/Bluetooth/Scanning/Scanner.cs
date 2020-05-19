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

namespace Covi.iOS.Features.Bluetooth.Scanning
{
    public class Scanner
    {
        private PeripheralManager _peripheralManager;
        private TracingInformation _tracingInformation;
        private ILogger<Scanner> _logger;
        private CBCentralManager _centralManager;
        private bool _enabled;

        public Scanner()
        {
            _logger = Covi.Logs.Logger.Get(this);

            _centralManager = new CBCentralManager(CoreFoundation.DispatchQueue.MainQueue);
            _centralManager.UpdatedState += HandleStateUpdated;
            _centralManager.DiscoveredPeripheral += HandleDiscoveredPeripheral;
            _centralManager.ConnectedPeripheral += HandleConnectedPeripheral;
            _centralManager.FailedToConnectPeripheral += HandleFailedToConnectPeripheral;
            _centralManager.DisconnectedPeripheral += HandleDisconnectedPeripheral;
        }

        private PeripheralManager GetPeripheralManager()
        {
            return _peripheralManager ??= new PeripheralManager(_centralManager, _tracingInformation);
        }

        private void HandleDisconnectedPeripheral(object sender, CBPeripheralErrorEventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                GetPeripheralManager().HandleDisconnectedPeripheral(e);
            }).FireAndForget();
        }

        private void HandleFailedToConnectPeripheral(object sender, CBPeripheralErrorEventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                GetPeripheralManager().HandleFailedToConnectPeripheral(e);
            }).FireAndForget();
        }

        private void HandleConnectedPeripheral(object sender, CBPeripheralEventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                GetPeripheralManager().HandleConnectedPeripheral(e);
            }).FireAndForget();
        }

        private void HandleDiscoveredPeripheral(object sender, CBDiscoveredPeripheralEventArgs e)
        {
            System.Threading.Tasks.Task.Run(() =>
            {
                Covi.Features.BluetoothTracing.TracingState.Instance.SetScanningState(true);
                GetPeripheralManager().HandleDiscoveredPeripheral(e);
            }).FireAndForget();
        }

        private void HandleStateUpdated(object sender, EventArgs e)
        {
            if (_centralManager.State == CBCentralManagerState.PoweredOn)
            {
                _logger.LogDebug("Scanner - restarting - " + _centralManager.State.ToString());
                if (_tracingInformation != null)
                {
                    Restart();
                }
            }
            else
            {
                Stop();
                _logger.LogDebug($"Scanning - stopped due to state {_centralManager.State.ToString()}.");
            }
        }

        private void Restart()
        {
            Stop();
            Start(_tracingInformation);
        }

        public void Start(TracingInformation config)
        {
            if (config == null || _enabled == true)
            {
                return;
            }

            _tracingInformation = config;

            CBUUID uuidService = CBUUID.FromString(_tracingInformation.ServiceId);
            CBUUID uuidCharacteristic = CBUUID.FromString(_tracingInformation.CharacteristicId);

            // TODO: options prevent android from being discovered on IOS, investigation required
            var options = new PeripheralScanningOptions()
            {
                AllowDuplicatesKey = true
            };

            _centralManager.ScanForPeripherals(new CBUUID[] { uuidService }, options);
            _enabled = true;

            Covi.Features.BluetoothTracing.TracingState.Instance.SetScanningState(true);
            _logger.LogDebug($"Scanning started for service {_tracingInformation.ServiceId}.");
        }

        public void Stop()
        {
            if (_centralManager == null)
            {
                // not started
                return;
            }

            _centralManager.StopScan();

            _logger.LogDebug("Scanning - stopped.");
            Covi.Features.BluetoothTracing.TracingState.Instance.SetScanningState(false);
            _enabled = false;
        }
    }
}
