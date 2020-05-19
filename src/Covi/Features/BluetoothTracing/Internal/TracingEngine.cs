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

using System.Threading.Tasks;
using Covi.Features.BluetoothTracing.TracingInformation;

namespace Covi.Features.BluetoothTracing.Internal
{
    public class TracingEngine : ITracingEngine
    {
        private readonly IPlatformTracingEngine _platformTracingEngine;

        public TracingEngine(IPlatformTracingEngine platformTracingEngine)
        {
            _platformTracingEngine = platformTracingEngine;
        }

        public async Task RestartAsync()
        {
            await _platformTracingEngine.RestartAsync().ConfigureAwait(false);
        }

        public void RecoverIfNeeded()
        {
            _platformTracingEngine.RecoverIfNeededAsync().FireAndForget();
        }

        public async Task<InitializationResult> SetupAsync(TracingInformation.TracingInformation tracingInformation)
        {
            await TracingInformationContainer.Instance.SetAsync(tracingInformation).ConfigureAwait(false);
            return await InitializePlatformAsync().ConfigureAwait(false);
        }

        public async Task StartAsync()
        {
            await _platformTracingEngine.StartAsync().ConfigureAwait(false);
        }

        public async Task StopAsync()
        {
            await _platformTracingEngine.StopAsync().ConfigureAwait(false);
        }

        private async Task<InitializationResult> InitializePlatformAsync()
        {
            return await _platformTracingEngine.InitializeAsync().ConfigureAwait(false);
        }
    }
}
