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
using Covi.Features.Account.Models;
using Covi.Services.Storage;

namespace Covi.Features.Account.Services
{
    public class AccountContainer : IAccountContainer
    {
        private const string StorageKeyName = "AccountInformation";
        private readonly Subject<Models.AccountInformation> _subject = new Subject<Models.AccountInformation>();
        private readonly IStorageService _storageService;

        public AccountContainer(IStorageService storageService)
        {
            _storageService = storageService;
            Changes = Observable.FromAsync(GetAsync)
                .Catch(Observable.Return<AccountInformation>(null))
                .Concat(_subject)
                .DistinctUntilChanged()
                .Synchronize()
                .Replay(1)
                .RefCount();
        }

        public IObservable<AccountInformation> Changes { get; }

        public async Task<AccountInformation> GetAsync()
        {
            return await _storageService.GetAsync<AccountInformation>(StorageKeyName).ConfigureAwait(false);
        }

        public async Task SetAsync(AccountInformation data)
        {
            await _storageService.AddAsync(StorageKeyName, data).ConfigureAwait(false);
            _subject.OnNext(data);
        }
    }

    public static class Roles
    {
        public const string Medical = "Medical";
        public const string User = "User";
    }
}
