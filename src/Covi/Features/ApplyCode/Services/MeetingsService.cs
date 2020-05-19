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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Covi.Features.BluetoothTracing;
using Covi.Client.Services.Platform.Models;
using Covi.Features.BluetoothTracing.TracesStorage;

namespace Covi.Features.ApplyCode.Services
{
    public class MeetingsService : IMeetingsService
    {
        private readonly ITracesStorage _traceStorage;

        public MeetingsService()
        {
            _traceStorage = TracesStorage.Instance;
        }

        public async Task<IReadOnlyCollection<Meeting>> GetMeetingsAsync()
        {
            var traces = await _traceStorage
                .QueryAsync(x => true).ConfigureAwait(false);
            var meetings = traces.Select(trace => new Meeting(trace.ContactToken, trace.ContactTimestamp))
                .ToList();

            return meetings;
        }
    }
}
