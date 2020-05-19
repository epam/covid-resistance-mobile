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
using Covi.Client.Services.Platform.Models;
using Covi.Services.Storage;

namespace Covi.Services.ApplicationMetadata
{
    public class MetadataContainer : IMetadataContainer
    {
        private const string StorageKeyName = "Metadata";
        private readonly IStorageService _storageService;
        private readonly Subject<Metadata> _subject = new Subject<Metadata>();

        public IObservable<Metadata> Changes { get; }

        public MetadataContainer(IStorageService storageService)
        {
            _storageService = storageService;
            Changes = Observable.FromAsync(GetAsync)
                .Catch(Observable.Return<Metadata>(null))
                .Concat(_subject)
                .DistinctUntilChanged()
                .Synchronize()
                .Replay(1)
                .RefCount();
        }

        public async Task<Metadata> GetAsync()
        {
            return await _storageService.GetAsync<Metadata>(StorageKeyName).ConfigureAwait(false);
        }

        public async Task SetAsync(Metadata data)
        {
            await _storageService.AddAsync(StorageKeyName, data).ConfigureAwait(false);
            _subject.OnNext(data);
        }
    }
}
