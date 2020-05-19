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

namespace Covi.Services.Serialization
{
    /// <summary>
    /// Provides serialization capabilities.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Serializes <paramref name="payload"/> of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Payload type.</typeparam>
        /// <param name="payload">Payload.</param>
        /// <returns>Serialized payload.</returns>
        Task<string> SerializeAsync<T>(T payload);

        /// <summary>
        /// Deserializes <paramref name="payload"/> into type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Payload type.</typeparam>
        /// <param name="payload">Serialized payload.</param>
        /// <returns>Deserialized payload.</returns>
        Task<T> DeserializeAsync<T>(string payload);
    }
}
