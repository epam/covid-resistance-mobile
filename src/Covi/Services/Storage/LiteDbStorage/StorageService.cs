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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiteDB;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace Covi.Services.Storage.LiteDbStorage
{
    /// <summary>
    /// LiteDB based <see cref="IStorageService"/> implementation.
    /// </summary>
    public class StorageService : IStorageService
    {
        private const string StorageFileName = "Storage.db";
        private static LiteDatabase _database;
        private readonly SemaphoreSlim _dbLock = new SemaphoreSlim(1);

        private readonly ILogger<StorageService> _logger;
        private readonly IStorageOptionsProvider _optionsProvider;

        public StorageService(IStorageOptionsProvider optionsProvider, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<StorageService>();
            _optionsProvider = optionsProvider;
        }

        private async Task<LiteDatabase> CreateAsync()
        {
            var options = await _optionsProvider.GetAsync();

            var filePath = GetFilePath();
            var connectionString = $"Filename={filePath}";

            if (!string.IsNullOrWhiteSpace(options.EncryptionKey))
            {
                var encryptionKey = options.EncryptionKey;
                connectionString += $";Password={encryptionKey}";
            }

            LiteDatabase database = null;
            try
            {
                database = new LiteDatabase(connectionString);
                Interlocked.CompareExchange(ref _database, database, null);
            }
            catch (Exception ex)
            {
                // In case of database opening errors, try to remove it and try again
                _logger.LogError(ex, "Failed to initialize the databse, trying to replace the file. Reason: " + ex.ToString());
                System.IO.File.Delete(filePath);
                database = new LiteDatabase(connectionString);
                Interlocked.CompareExchange(ref _database, database, null);
            }

            return database;
        }

        protected static string GetFilePath()
        {
            var directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return System.IO.Path.Combine(directoryPath, StorageFileName);
        }

        public async Task AddAsync<T>(string key, T data)
            where T : class
        {
            await Task.Run(async () =>
            {
                using (await _dbLock.LockAsync())
                {
                    using (var db = await CreateAsync())
                    {
                        var collection = db.GetCollection<Entry>(key);
                        var serializedData = await Serialization.Serializer.Instance.SerializeAsync(data).ConfigureAwait(false);
                        var entry = new Entry()
                        {
                            Id = key,
                            Payload = serializedData,
                        };
                        collection.Upsert(entry);
                    }
                }
            }).ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T>(string key)
            where T : class
        {
            return await Task.Run(async () =>
            {
                using (await _dbLock.LockAsync())
                {
                    using (var db = await CreateAsync())
                    {
                        var result = default(T);
                        var collection = db.GetCollection<Entry>(key);
                        var entry = collection.FindAll().FirstOrDefault();
                        if (!string.IsNullOrEmpty(entry?.Payload))
                        {
                            try
                            {
                                var deserialized = await Serialization.Serializer.Instance.DeserializeAsync<T>(entry.Payload).ConfigureAwait(false);
                                result = deserialized;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Failed to deserialize the {key} entity.");
                            }
                        }

                        return result;
                    }
                }
            }).ConfigureAwait(false);
        }

        public async Task ClearAsync(string key)
        {
            await Task.Run(async () =>
            {
                using (await _dbLock.LockAsync())
                {
                    using (var db = await CreateAsync())
                    {
                        db.GetCollection(key).DeleteAll();
                    }
                }
            }).ConfigureAwait(false);
        }

        public void Suspend()
        {
        }
    }
}
