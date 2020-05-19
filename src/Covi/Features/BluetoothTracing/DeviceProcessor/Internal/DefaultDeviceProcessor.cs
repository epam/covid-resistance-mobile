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
using System.Threading;
using System.Threading.Tasks;
using Covi.Features.BluetoothTracing.TracesStorage;
using Microsoft.Extensions.Logging;

namespace Covi.Features.BluetoothTracing.DeviceProcessor.Internal
{
    public class DefaultDeviceProcessor : IDeviceProcessor
    {
        private readonly ITracesStorage _tracesStorage;
        private ILogger _logger;
        private object _cleanupLock = new object();
        private DateTime _lastCleanupTime = DateTime.UtcNow;

        public DefaultDeviceProcessor()
        {
            _logger = Covi.Logs.Logger.Get(this);
            _tracesStorage = TracesStorage.TracesStorage.Instance;
        }

        public void Process(ContactDescriptor contact)
        {
            _logger.LogDebug(
                        $"DeviceProcessor - processing deviceId: {contact.DeviceToken}.");
            var trace = new Trace()
            {
                ContactTimestamp = contact.ContactTimestamp,
                ContactToken = contact.DeviceToken
            };
            _tracesStorage.AddAsync(trace);
            InvokeCleanupIfNeeded();
        }

        private void InvokeCleanupIfNeeded()
        {
            if (((DateTime.UtcNow - _lastCleanupTime) <= Configuration.Constants.BluetoothConstants.DeviceThrottleTime))
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
                    if (((DateTime.UtcNow - _lastCleanupTime) <= Configuration.Constants.BluetoothConstants.DeviceThrottleTime))
                    {
                        return;
                    }

                    _logger.LogDebug($"DeviceProcessor - Cleanup started.");
                    var minPurgeDate = DateTime.UtcNow - Configuration.Constants.BluetoothConstants.DataExpirationTime;
                    _tracesStorage.Purge(minPurgeDate);
                    _lastCleanupTime = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"DeviceProcessor -  Cleanup failed.");
                }
                finally
                {
                    Monitor.Exit(_cleanupLock);
                }
            });
        }
    }
}
