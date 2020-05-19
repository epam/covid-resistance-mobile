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
using System.Threading.Tasks;

namespace Covi.Services.Serialization
{
    public static class Serializer
    {
        private static ISerializer _instance;

        private static readonly Lazy<ISerializer> DefaultInstance =
            new Lazy<ISerializer>(() => new DefaultSerializer());

        /// <summary>
        /// Gets the current serializer instance.
        /// </summary>
        public static ISerializer Instance
        {
            get { return _instance ??= DefaultInstance.Value; }
        }

        public static void Setup(ISerializer serializer)
        {
            _instance = serializer;
        }

        public class DefaultSerializer : ISerializer
        {
            public Task<string> SerializeAsync<T>(T payload)
            {
                var result = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
                return Task.FromResult(result);
            }

            public Task<T> DeserializeAsync<T>(string payload)
            {
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(payload);
                return Task.FromResult(result);
            }
        }
    }
}
