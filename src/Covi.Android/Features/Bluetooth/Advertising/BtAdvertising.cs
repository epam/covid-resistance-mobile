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

using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.OS;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.TracingInformation;

namespace Covi.Droid.Features.Bluetooth.Advertising
{
    public abstract class BtAdvertising
    {
        protected BtAdvertising()
        {
        }

        public BluetoothLeAdvertiser Advertiser { get; private set; }
        public bool Initialized { get; private set; }

        public static BtAdvertising CreateAdvertiser()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                return new BtAdvertisingForO();
            }
            else
            {
                return new BtAdvertisingForM();
            }
        }

        public void Init()
        {
            Advertiser = BluetoothAdapter.DefaultAdapter?.BluetoothLeAdvertiser;
            Initialized = Advertiser != null;
        }

        public abstract void StartAdvertising(TracingInformation tracingInformation);

        public abstract void StopAdvertising();

        protected AdvertiseData BuildAdvertiseData(TracingInformation tracingInformation)
        {
            AdvertiseData.Builder dataBuilder = new AdvertiseData.Builder();
            var uuid = ParcelUuid.FromString(tracingInformation.ServiceId);
            dataBuilder.AddServiceUuid(uuid);
            return dataBuilder.Build();
        }
    }
}
