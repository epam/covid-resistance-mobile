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

using Android.Bluetooth.LE;
using Microsoft.Extensions.Logging;

namespace Covi.Droid.Features.Bluetooth.Advertising
{
    public class BtAdvertisingSetCallback : AdvertisingSetCallback
    {
        private readonly ILogger _logger;

        public BtAdvertisingSetCallback()
        {
            _logger = Covi.Logs.Logger.Get(this);
        }

        public override void OnAdvertisingSetStarted(AdvertisingSet advertisingSet, int txPower, AdvertiseResult status)
        {
            base.OnAdvertisingSetStarted(advertisingSet, txPower, status);
            _logger.LogDebug($"Advertiser - sending, status: {status}.");

        }

        public override void OnScanResponseDataSet(AdvertisingSet advertisingSet, AdvertiseResult status)
        {
            base.OnScanResponseDataSet(advertisingSet, status);

            _logger.LogDebug($"Advertiser - responding to scan, status: {status}.");
        }

    }
}
