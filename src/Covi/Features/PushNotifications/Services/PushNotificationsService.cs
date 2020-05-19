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

using Covi.Client.Services;
using Covi.Client.Services.Platform.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Covi.Features.PushNotifications.Services
{
    public class PushNotificationsService : IPushNotificationsService
    {
        private readonly IPlatformClient _platformClient;
        private readonly ILogger<NotificationInfo> _logger;

        private readonly ConcurrentBag<IPushNotificationInitializer> _initializers = new ConcurrentBag<IPushNotificationInitializer>();
        private string _lastSentToken = string.Empty;
        private bool _isFirstRun = true;

        public PushNotificationsService(
            IPlatformClient platformClient,
            ILoggerFactory loggerFactory)
        {
            _platformClient = platformClient;
            _logger = loggerFactory.CreateLogger<NotificationInfo>();
        }

        public async Task UpdateTokenAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var isInitialized = await InitializeAsync();
                    if (!isInitialized)
                    {
                        return;
                    }

                    var pushNotificationToken =
                        await PushNotificationsContainer.Instance.GetAsync().ConfigureAwait(false);

                    // Delay initialization to give time for the token to be generated if it is the first run
                    if (string.IsNullOrWhiteSpace(pushNotificationToken?.PushNotificationToken) && _isFirstRun)
                    {
                        await Task.Delay(5000).ConfigureAwait(false);
                        pushNotificationToken = await PushNotificationsContainer.Instance.GetAsync().ConfigureAwait(false);
                    }

                    _isFirstRun = false;

                    if (pushNotificationToken == null)
                    {
                        return;
                    }

                    var currentToken = pushNotificationToken.PushNotificationToken;

                    //Send token only once per session or if changed
                    if (!string.Equals(_lastSentToken, currentToken, StringComparison.OrdinalIgnoreCase))
                    {
                        var profileResponse = await _platformClient.Endpoints.SetupNotificationsWithHttpMessagesAsync(pushNotificationToken).ConfigureAwait(false);
                        _lastSentToken = currentToken;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update push notification token for the device");
                }
            }).ConfigureAwait(false);
        }

        public void AddInitializer(IPushNotificationInitializer pushNotificationInitializer)
        {
            if (pushNotificationInitializer != null)
            {
                _initializers.Add(pushNotificationInitializer);
            }
        }

        private async Task<bool> InitializeAsync()
        {
            var initializers = _initializers.ToList();
            var result = true;

            foreach (var initializer in initializers)
            {
                try
                {
                    result &= await initializer.InitializeAsync();
                    if (!result)
                    {
                        break;
                    }
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
