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
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Covi.Features.BluetoothTracing.TracingInformation.Internal
{
    public class DefaultTracingInformationContainer : ITracingInformationContainer
    {

        private const string TracingInformationStorageKey = "TracingInformationKey";
        private Subject<TracingInformation> _changesSubject = new Subject<TracingInformation>();

        public DefaultTracingInformationContainer()
        {
            Changes = Observable.FromAsync(GetAsync).Concat(_changesSubject).DistinctUntilChanged().Replay(1).RefCount();
        }

        public IObservable<TracingInformation> Changes { get; }

        public async Task SetAsync(TracingInformation tracingInformation)
        {
            var serialized = await Services.Serialization.Serializer.Instance.SerializeAsync(tracingInformation).ConfigureAwait(false);
            await Xamarin.Essentials.SecureStorage.SetAsync(TracingInformationStorageKey, serialized).ConfigureAwait(false);
            _changesSubject.OnNext(tracingInformation);
        }

        public async Task<TracingInformation> GetAsync()
        {
            TracingInformation result = null;
            try
            {
                var data = await Xamarin.Essentials.SecureStorage.GetAsync(TracingInformationStorageKey).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(data))
                {
                    result = await Services.Serialization.Serializer.Instance.DeserializeAsync<TracingInformation>(data);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Failed to deserialize tracing information. " + ex.ToString());
            }

            return result ?? TracingInformation.Default;
        }
    }
}
