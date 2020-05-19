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
using Covi.Services.Dispatcher;
using UserNotifications;

namespace Covi.iOS.Features.PushNotifications
{
    public class PushNotificationPermissionInitializer : Covi.Features.PushNotifications.IPushNotificationInitializer
    {
        private const int InitializationDelayMilliseconds = 20000;
        private readonly IDispatcherService _dispatcher;

        public PushNotificationPermissionInitializer(IDispatcherService dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task<bool> InitializeAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();

            await _dispatcher.InvokeAsync(() =>
            {
                UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound,
                    (isAllowed, error) =>
                    {
                        taskCompletionSource.TrySetResult(isAllowed);
                    });
            });

            var result = await taskCompletionSource.Task.ReturnInTimeoutAsync(false, InitializationDelayMilliseconds);
            return result;
        }
    }
}
