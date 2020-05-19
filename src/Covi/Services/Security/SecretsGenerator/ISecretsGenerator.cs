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

namespace Covi.Services.Security.SecretsGenerator
{
    /// <summary>
    /// Securely generates secrets.
    /// </summary>
    public interface ISecretsGenerator
    {
        /// <summary>
        /// Generate secret key.
        /// </summary>
        /// <returns>Encoded key.</returns>
        string Generate();

        /// <summary>
        /// Generate device identifier.
        /// </summary>
        /// <returns>Encoded device identifier.</returns>
        string GenerateDeviceId();
    }
}
