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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Covi.Services.Storage.LiteDbStorage;
using LiteDB;
using Nito.AsyncEx;

namespace Covi.Features.BluetoothTracing.TracesStorage.LiteDbStorage
{
    public class LiteDbTracesStorage : ITracesStorage
    {
        private const string FileName = "Traces.db";
        private readonly string _connectionString;
        private readonly SemaphoreSlim _dbLock = new SemaphoreSlim(1);

        public LiteDbTracesStorage(StorageOptions options)
        {
            var filePath = GetFilePath();
            var connectionString = $"Filename={filePath};Mode=Shared";

            if (!string.IsNullOrWhiteSpace(options.EncryptionKey))
            {
                var encryptionKey = options.EncryptionKey;
                connectionString += $";Password={encryptionKey}";
            }

            _connectionString = connectionString;
        }

        protected virtual string GetFilePath()
        {
            var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return System.IO.Path.Combine(directoryPath, FileName);
        }

        public async Task AddAsync(Trace trace)
        {
            using (await _dbLock.LockAsync())
            {
                using (var repository = new LiteRepository(_connectionString))
                {
                    repository.Insert(trace);
                }
            }
        }

        public async Task<IReadOnlyCollection<Trace>> QueryAsync(System.Linq.Expressions.Expression<Func<Trace, bool>> predicate)
        {
            using (await _dbLock.LockAsync())
            {
                using (var repository = new LiteRepository(_connectionString))
                {
                    var entries = repository.Query<Trace>().Where(predicate).ToList();
                    return (IReadOnlyCollection<Trace>)entries;
                }
            }
        }

        public async Task Purge(DateTime minimumDate)
        {
            using (await _dbLock.LockAsync())
            {
                using (var repository = new LiteRepository(_connectionString))
                {
                    repository.DeleteMany<Trace>(x => x.ContactTimestamp <= minimumDate);
                }
            }
        }

        public async Task ClearAsync()
        {
            using (await _dbLock.LockAsync())
            {
                using (var repository = new LiteRepository(_connectionString))
                {
                    repository.DeleteMany<Trace>((x) => true);
                }
            }
        }
    }
}
