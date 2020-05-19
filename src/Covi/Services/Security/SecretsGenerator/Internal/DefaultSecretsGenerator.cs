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
using System.Security.Cryptography;

namespace Covi.Services.Security.SecretsGenerator.Internal
{
    /// <summary>
    /// Default implementation of <see cref="ISecretsGenerator"/>.
    /// </summary>
    public class DefaultSecretsGenerator : ISecretsGenerator
    {
        public string Generate()
        {
            var aes = new AesCryptoServiceProvider();
            aes.GenerateIV();
            aes.GenerateKey();
            var encodedKey = Convert.ToBase64String(aes.Key);
            return encodedKey;
        }

        public string GenerateDeviceId()
        {
            return System.Guid.NewGuid().ToString("N");
        }
    }
}
