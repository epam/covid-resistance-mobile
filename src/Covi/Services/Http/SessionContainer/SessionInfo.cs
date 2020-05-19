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
using Covi.Client.Services.Platform.Models;
using Newtonsoft.Json;

namespace Covi.Services.Http.SessionContainer
{
    [Serializable]
    public class SessionInfo
    {
        [JsonProperty(PropertyName = "accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty(PropertyName = "refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty(PropertyName = "expirationDate")]
        public DateTime ExpirationDate { get; set; }

        public static SessionInfo CreateFromToken(Token token)
        {
            if (token == null)
            {
                return null;
            }

            var expirationDate = DateTime.Now;
            return new SessionInfo
            {
                RefreshToken = token.RefreshToken,
                AccessToken = token.AccessToken,
                ExpirationDate = expirationDate.AddSeconds(token.ExpiresIn ?? 0)
            };
        }
    }
}
