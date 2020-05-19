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

namespace Covi.Features.BluetoothTracing
{
    // TODO: implement background service status reporting
    public static class TracingState
    {
        private static Lazy<StubState> _stubState = new Lazy<StubState>();
        private static Func<ITracingState> _stateProvider;

        public static ITracingState Instance
        {
            get => _stateProvider != null ? _stateProvider.Invoke() : _stubState.Value;
        }

        public static void SetStateProvider(Func<ITracingState> stateProvider)
        {
            _stateProvider = stateProvider;
        }

        private class StubState : ITracingState
        {
            public bool IsAdvertising { get; private set; }

            public DateTime StateUpdateTimestamp { get; set; }

            public bool IsScanning { get; private set; }

            public void SetAdvertisingState(bool isAdvertising)
            {
                IsAdvertising = isAdvertising;
                StateUpdateTimestamp = DateTime.UtcNow;
            }

            public void SetScanningState(bool isScanning)
            {
                IsScanning = isScanning;
                StateUpdateTimestamp = DateTime.UtcNow;
            }
        }
    }

    public interface ITracingState
    {
        bool IsAdvertising { get; }

        DateTime StateUpdateTimestamp { get; }

        bool IsScanning { get; }

        void SetAdvertisingState(bool isAdvertising);

        void SetScanningState(bool isScanning);
    }

    public class BtDevice
    {
        private string _macAddress;
        private long _lastSeen;

        public BtDevice(string macAddress, long lastSeen)
        {
            this._macAddress = macAddress;
            this._lastSeen = lastSeen;
        }

        public string MacAddress { get => _macAddress; }

        public long LastSeen { get => _lastSeen; }

        public override bool Equals(object obj)
        {
            return obj is BtDevice device &&
                   _macAddress == device._macAddress;
        }

        public override int GetHashCode()
        {
            return _macAddress.GetHashCode();
        }
    }
}
