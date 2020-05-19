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
using Covi.Services.Dispatcher;
using Xamarin.Forms;

namespace Covi.Services.Notification
{
    public class NotificationManager : INotificationManager
    {
        private readonly IDispatcherService _dispatcher;

        public NotificationManager(IDispatcherService dispatcherService)
        {
            _dispatcher = dispatcherService;
        }

        public Task<bool> ShowNotificationAsync(
            string title,
            string description,
            string accept,
            string cancel)
        {
            var tcs = new TaskCompletionSource<bool>();

            _dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    bool displayAlertResult;
                    if (string.IsNullOrWhiteSpace(cancel))
                    {
                        await Application.Current.MainPage.DisplayAlert(title, description, accept);
                        displayAlertResult = true;
                    }
                    else
                    {
                        displayAlertResult =
                            await Application.Current.MainPage.DisplayAlert(title, description, accept, cancel);
                    }

                    tcs.SetResult(displayAlertResult);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });

            return tcs.Task;
        }

        public Task<bool> ShowNotificationAsync(
            string title,
            string description,
            string accept)
        {
            return ShowNotificationAsync(title, description, accept, string.Empty);
        }
    }
}
