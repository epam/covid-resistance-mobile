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
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Util;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.TracingInformation;
using Microsoft.Extensions.Logging;

namespace Covi.Droid.Features.Bluetooth.Gatt
{
    public class BtGattServer
    {
        private BluetoothGattServer _gattServer;
        private BluetoothManager _bluetoothManager;
        private TracingInformation _tracingInformation;
        private readonly ILogger _logger;

        public BtGattServer()
        {
            _logger = Covi.Logs.Logger.Get(this);
        }

        public bool Initialized { get; private set; }

        public void Init(Context context)
        {
            _bluetoothManager = (BluetoothManager)context.GetSystemService(Context.BluetoothService);
            Initialized = _bluetoothManager != null;
        }

        public void Start(Context context, TracingInformation tracingInformation)
        {
            if (!Initialized)
            {
                _logger.LogError("Advertiser - Starting failed - not initialized.");
                return;
            }

            _tracingInformation = tracingInformation;

            try
            {
                _logger.LogDebug("Advertiser - starting.");

                var serviceUuid = ParcelUuid.FromString(_tracingInformation.ServiceId);
                var characteristicUuid = ParcelUuid.FromString(_tracingInformation.CharacteristicId);

                _logger.LogDebug($"Advertiser - initializing. ServiceId: {_tracingInformation.ServiceId}. CharacteristicId: {_tracingInformation.CharacteristicId}");

                BtGattServerCallback bluetoothGattServerCallback = new BtGattServerCallback(_tracingInformation);
                _gattServer = _bluetoothManager.OpenGattServer(context, bluetoothGattServerCallback);
                bluetoothGattServerCallback._gattServer = _gattServer;

                BluetoothGattService service = new BluetoothGattService(serviceUuid.Uuid, GattServiceType.Primary);
                BluetoothGattCharacteristic characteristic = new BluetoothGattCharacteristic(characteristicUuid.Uuid, GattProperty.Read, GattPermission.Read);
                service.AddCharacteristic(characteristic);
                _gattServer.AddService(service);

                _logger.LogDebug("Advertiser - started.");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Advertiser - failed advertising initialization.");
            }
        }

        public void Stop()
        {
            if (!Initialized)
            {
                return;
            }
            try
            {
                _logger.LogDebug($"Advertiser - stopping.");
                _bluetoothManager = null;
                _gattServer.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Advertiser - Stopping failed.");
            }

        }
    }
}
