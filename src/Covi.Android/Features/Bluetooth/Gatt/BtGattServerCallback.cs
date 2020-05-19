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

using System.Linq;
using Android.Bluetooth;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.TracingInformation;
using Microsoft.Extensions.Logging;

namespace Covi.Droid.Features.Bluetooth.Gatt
{
    public class BtGattServerCallback : BluetoothGattServerCallback
    {
        internal BluetoothGattServer _gattServer;
        private readonly TracingInformation _tracingInformation;
        private readonly ILogger _logger;

        public BtGattServerCallback(TracingInformation tracingInformation)
        {
            _tracingInformation = tracingInformation;
            _logger = Logs.Logger.Get(this);
        }

        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);
            TracingState.Instance.SetAdvertisingState(true);
        }

        public override void OnCharacteristicReadRequest(
            BluetoothDevice device,
            int requestId,
            int offset,
            BluetoothGattCharacteristic characteristic)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, characteristic);
            if (_gattServer == null)
            {
                return;
            }

            var response = PayloadFormatter
                .GetBytesToSend(new PackageData(_tracingInformation.DeviceId))
                .Skip(offset)
                .ToArray();

            _logger.LogDebug($"Advertiser - Device connected. Sending response to {device.Address}. Offset={offset}, ResponseLength={response.Length}.");
            _gattServer.SendResponse(device, requestId, GattStatus.Success, offset, response);
        }
    }
}
