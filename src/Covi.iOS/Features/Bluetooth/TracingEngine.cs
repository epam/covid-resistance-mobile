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
using System.Threading.Tasks;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.TracingInformation;
using Covi.iOS.Features.Bluetooth.Advertising;
using Covi.iOS.Features.Bluetooth.Scanning;
using Microsoft.Extensions.Logging;

namespace Covi.iOS.Features.Bluetooth
{
    public class TracingEngine : IPlatformTracingEngine
    {
        private static TracingEngine _instance;

        private Advertiser _advertiser;
        private Scanner _scanner;
        private ILogger _logger;
        private TracingInformation _config;

        public static TracingEngine Instance
        {
            get => _instance ??= new TracingEngine();
        }

        public TracingEngine()
        {
            _logger = Covi.Logs.Logger.Get(this);
        }

        public async Task<InitializationResult> InitializeAsync()
        {
            var initializer = new PlatformInitializer();
            var result = await initializer.InitializeAsync();
            return result;
        }

        public async Task StartAsync()
        {
            _config = await TracingInformationContainer.Instance.GetAsync().ConfigureAwait(false);
            if (!_config.Enabled)
            {
                return;
            }

            _advertiser ??= new Advertiser();
            _scanner ??= new Scanner();

            _advertiser.Start(_config);
            _scanner.Start(_config);
        }

        public Task StopAsync()
        {
            _advertiser?.Stop();
            _scanner?.Stop();
            return Task.CompletedTask;
        }

        public async Task RestartAsync()
        {
            await StopAsync();
            await StartAsync();
        }

        public async Task RecoverIfNeededAsync()
        {

            try
            {
                var isAdvertising = Covi.Features.BluetoothTracing.TracingState.Instance.IsAdvertising;
                var isScanning = Covi.Features.BluetoothTracing.TracingState.Instance.IsScanning;
                var isScanningHeartbeatOutdated =
                    Covi.Features.BluetoothTracing.TracingState.Instance.StateUpdateTimestamp <=
                    DateTime.UtcNow.AddMinutes(5);
                if (!isAdvertising ||
                    !isScanning ||
                    isScanningHeartbeatOutdated)
                {
                    await RestartAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to restart tracing engine.");
            }
        }
    }
}
