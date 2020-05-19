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

namespace Covi.Services.Http.SessionContainer
{
    public class SessionContainer : ISessionContainer
    {
        private const string _sessionInfoKey = "SessionInfoKey";

        public async Task AddOrReplaceAsync(SessionInfo sessionInfo)
        {
            if (sessionInfo == null)
            {
                return;
            }

            var serializedString = await Serialization.Serializer.Instance
                .SerializeAsync(sessionInfo)
                .ConfigureAwait(false);
            await Xamarin.Essentials.SecureStorage
                .SetAsync(_sessionInfoKey, serializedString)
                .ConfigureAwait(false);
        }

        public async Task<SessionInfo> GetSessionInfoAsync()
        {
            try
            {
                var serializedString = await Xamarin.Essentials.SecureStorage
                    .GetAsync(_sessionInfoKey)
                    .ConfigureAwait(false);
                var sessionInfo = await Serialization.Serializer.Instance
                    .DeserializeAsync<SessionInfo>(serializedString)
                    .ConfigureAwait(false);
                return sessionInfo;
            }
            catch
            {
                return null;
            }
        }
    }
}
