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
using System.Text;

namespace Covi.Features.BluetoothTracing
{
    public static class PayloadFormatter
    {
        public static byte[] GetBytesToSend(PackageData package)
        {
            var deviceId = package.DeviceId;
            if (string.IsNullOrEmpty(deviceId))
            {
                return new byte[]{};
            }

            var payloadBytes = Encoding.UTF8.GetBytes(deviceId);
            var payloadLengthBytes = BitConverter.GetBytes(payloadBytes.Length);
            payloadBytes = payloadLengthBytes.Concat(payloadBytes).ToArray();

            return payloadBytes;
        }

        public static bool TryGetValue(byte[] bytes, out PackageData value)
        {
            if (bytes?.Any() != true)
            {
                value = PackageData.Empty;
                return false;
            }

            var sizeFlagBytes = sizeof(int);
            var length = BitConverter.ToInt32(bytes.Take(sizeFlagBytes).ToArray(), 0);
            if (bytes.Length - sizeFlagBytes != length)
            {
                value = PackageData.Empty;
                return false;
            }

            var encoded = Encoding.UTF8.GetString(bytes.Skip(sizeFlagBytes).ToArray());
            value = new PackageData(encoded);
            return true;
        }
    }

    public struct PackageData
    {
        public PackageData(string deviceId)
        {
            DeviceId = deviceId;
        }

        public string DeviceId { get; }

        public static PackageData Empty => default(PackageData);
    }
}
