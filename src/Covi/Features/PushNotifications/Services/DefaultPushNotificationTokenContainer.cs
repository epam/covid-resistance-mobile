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

using Covi.Client.Services.Platform.Models;
using Covi.Features.PushNotifications.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Covi.Features.PushNotifications.Services
{
    public class DefaultPushNotificationTokenContainer : IPushNotificationsContainer
    {
        private const string PushNotificationTokenStorageKey = "PushNotificationTokenKey";

        public async Task SetAsync(PushNotificationTokenModel data)
        {
            var serialized = JsonConvert.SerializeObject(
                new NotificationInfo(data.Token, data.Type));
            await Xamarin.Essentials.SecureStorage.SetAsync(PushNotificationTokenStorageKey, serialized)
                .ConfigureAwait(false);
        }

        public async Task<NotificationInfo> GetAsync()
        {
            var logger = Logs.Logger.Get(this);

            NotificationInfo result = null;
            var pushNotificationToken = await Xamarin.Essentials.SecureStorage.GetAsync(PushNotificationTokenStorageKey)
                .ConfigureAwait(false);
            if (!string.IsNullOrEmpty((pushNotificationToken)))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<NotificationInfo>(pushNotificationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to deserialize push notifications token.");
                }
            }

            return result;
        }
    }
}
