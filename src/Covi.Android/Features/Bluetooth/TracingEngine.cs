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
using Android.App;
using Android.Content;
using Android.OS;
using Covi.Droid.Features.Services;
using Covi.Features.BluetoothTracing;
using Microsoft.Extensions.Logging;

namespace Covi.Droid.Features.Bluetooth
{
    public class TracingEngine : IPlatformTracingEngine
    {
        private static TracingEngine _instance;
        private ILogger<TracingEngine> _logger;

        public TracingEngine()
        {
            _logger = Covi.Logs.Logger.Get(this);
        }

        public static TracingEngine Instance
        {
            get => _instance ??= new TracingEngine();
        }

        public async Task<InitializationResult> InitializeAsync()
        {
            var initializer = new PlatformInitializer();
            var result = await initializer.InitializeAsync();
            return result;
        }

        public Task RecoverIfNeededAsync()
        {
            var service = BtService.Instance;
            if (service == null || service.Initialized == false || service.Running == false)
            {
                StartAsync();
            }
            return Task.CompletedTask;
        }

        public async Task RestartAsync()
        {
            await StopAsync();
            await StartAsync();
        }

        public Task StartAsync()
        {
            var service = BtService.Instance;
            if (service == null || service.Initialized == false)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    Covi.Droid.Features.Services.JobService.EnqueueWork(Application.Context, new Intent());
                }
                else
                {
                    Application.Context.StartService(BtService.GetServiceIntent(Application.Context));
                }
            }
            else
            {
                service.Start();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            var service = BtService.Instance;
            if (service != null || service.Running == true)
            {
                service.Stop();
            }

            return Task.CompletedTask;
        }
    }
}
