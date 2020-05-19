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

namespace Covi.Services.Security.SecretsProvider
{
    /// <summary>
    /// Provides an access to an application secrets.
    /// </summary>
    public interface ISecretsProvider
    {
        /// <summary>
        /// Initializes secrets.
        /// </summary>
        Task InitializeAsync();

        /// <summary>
        /// Initializes secrets with provided deviceId.
        /// </summary>
        Task InitializeAsync(string deviceId);

        /// <summary>
        /// Gets an application wide encryption key.
        /// </summary>
        Task<string> GetEncryptionKeyAsync();

        /// <summary>
        /// Gets device identifier.
        /// </summary>
        Task<string> GetDeviceIdentifierAsync();

        /// <summary>
        /// Gets a flag that indicates whether secrets has been initialized.
        /// </summary>
        bool GetIsInitialized();
    }
}
