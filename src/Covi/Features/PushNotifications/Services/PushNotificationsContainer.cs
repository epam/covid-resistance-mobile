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

namespace Covi.Features.PushNotifications.Services
{
    public class PushNotificationsContainer
    {
        private static IPushNotificationsContainer _instance;

        private static readonly Lazy<IPushNotificationsContainer> DefaultInstance =
            new Lazy<IPushNotificationsContainer>(() => new DefaultPushNotificationTokenContainer());

        /// <summary>
        /// Gets the current push notification token container instance.
        /// </summary>
        public static IPushNotificationsContainer Instance
        {
            get { return _instance ??= DefaultInstance.Value; }
        }

        public static void Setup(IPushNotificationsContainer container)
        {
            _instance = container;
        }
    }
}
