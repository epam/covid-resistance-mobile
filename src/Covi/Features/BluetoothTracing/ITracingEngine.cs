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
using Xamarin.Essentials;

namespace Covi.Features.BluetoothTracing
{
    public interface ITracingEngine
    {
        Task<InitializationResult> SetupAsync(TracingInformation.TracingInformation tracingInformation);
        Task StartAsync();
        Task StopAsync();
        Task RestartAsync();
        void RecoverIfNeeded();
    }

    public class InitializationResult
    {
        public PermissionStatus PermissionStatus { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
