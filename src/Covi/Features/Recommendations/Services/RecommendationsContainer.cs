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

namespace Covi.Features.Recommendations.Services
{
    public class RecommendationsContainer : IRecommendationsContainer
    {
        private const string StorageKeyName = "RecommendationsList";
        private readonly IStorageService _storageService;
        private readonly BehaviorSubject<RecommendationsList> _subject = new BehaviorSubject<RecommendationsList>(null);

        public IObservable<RecommendationsList> Changes { get; }

        public RecommendationsContainer(IStorageService storageService)
        {
            _storageService = storageService;

            Changes = _subject.Merge(Observable.FromAsync(GetAsync))
                               .Catch(Observable.Return<RecommendationsList>(null))
                               .Synchronize()
                               .DistinctUntilChanged()
                               .Replay(1)
                               .RefCount();
        }

        public async Task<RecommendationsList> GetAsync()
        {
            return await _storageService.GetAsync<RecommendationsList>(StorageKeyName).ConfigureAwait(false);
        }

        public Task SetAsync(RecommendationsList data)
        {
            _subject.OnNext(data);
            return Task.CompletedTask;
        }
    }
}
