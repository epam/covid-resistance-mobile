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
using Android.Bluetooth.LE;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.TracingInformation;
using Microsoft.Extensions.Logging;

namespace Covi.Droid.Features.Bluetooth.Advertising
{
    public class BtAdvertisingForO : BtAdvertising
    {
        private BtAdvertisingSetCallback _callback;
        private ILogger _logger;

        public BtAdvertisingForO()
        {
            _logger = Covi.Logs.Logger.Get(this);
        }

        public TracingInformation TracingInformation { get; private set; }

        public override void StartAdvertising(TracingInformation tracingInformation)
        {
            if (!Initialized)
            {
                _logger.LogError("Advertiser - Starting failed - not initialized.");
                return;
            }

            TracingInformation = tracingInformation;

            try
            {
                _logger.LogDebug($"Advertiser - Starting. ServiceId: {tracingInformation.ServiceId}.");
                AdvertisingSetParameters parameters = AdvertisingParameters();
                AdvertiseData data = BuildAdvertiseData(tracingInformation);
                _callback = new BtAdvertisingSetCallback();
                System.Threading.Tasks.Task.Run(() =>
                {
                    Advertiser.StartAdvertisingSet(parameters, data, null, null, null, _callback);
                });
                Covi.Features.BluetoothTracing.TracingState.Instance.SetAdvertisingState(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Advertiser - Starting failed.");
            }
        }

        public override void StopAdvertising()
        {
            if (!Initialized)
            {
                return;
            }
            try
            {
                _logger.LogDebug("Advertiser - Stopping.");
                Advertiser.StopAdvertisingSet(_callback);
                Covi.Features.BluetoothTracing.TracingState.Instance.SetAdvertisingState(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Advertiser - Stopping failed.");
            }
        }

        private AdvertisingSetParameters AdvertisingParameters()
        {
            AdvertisingSetParameters.Builder builder = new AdvertisingSetParameters.Builder();
            return builder.SetLegacyMode(true)
                          .SetConnectable(true)
                          .SetScannable(true)
                          .SetInterval(AdvertisingSetParameters.IntervalMedium)
                          .SetTxPowerLevel(AdvertiseTxPower.Low)
                          .Build();
        }
    }
}
