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

namespace Covi.Configuration
{
    public static class Constants
    {
        public static void Initialize(IEnvironmentConfiguration environmentConfiguration)
        {
            BluetoothConstants.ServiceUUID = environmentConfiguration.GetValue("BluetoothTracing_ServiceUUID");
            BluetoothConstants.CharacteristicUUID = environmentConfiguration.GetValue("BluetoothTracing_CharacteristicUUID");
            BluetoothConstants.DataExpirationTime = TimeSpan.FromDays(int.Parse(environmentConfiguration.GetValue("BluetoothTracing_DataExpirationTimeDays")));
            BluetoothConstants.DeviceThrottleTime = TimeSpan.FromMinutes(int.Parse(environmentConfiguration.GetValue("BluetoothTracing_DeviceThrottleTimeMinutes")));

            PushNotificationsConstants.SubscriptionTags = new string[] { environmentConfiguration.GetValue("PushNotifications_SubscriptionTags") };
            PushNotificationsConstants.FCMTemplateBody = environmentConfiguration.GetValue("PushNotifications_FCMTemplateBody");
            PushNotificationsConstants.APNTemplateBody = environmentConfiguration.GetValue("PushNotifications_APNTemplateBody");
        }

        public static class PlatformConstants
        {
            public static readonly string EndpointUrl = Configuration.Helpers.Secrets.EndpointUrl;
        }

        public static class BluetoothConstants
        {
            public static string ServiceUUID { get; internal set; }
            public static string CharacteristicUUID { get; internal set; }

            public static TimeSpan DataExpirationTime { get; internal set; }
            public static TimeSpan DeviceThrottleTime { get; internal set; }
        }

        public static class AppCenterConstants
        {
            public static readonly string Secret_iOS = Configuration.Helpers.Secrets.AppCenter_iOS_Secret;
            public static readonly string Secret_Android = Configuration.Helpers.Secrets.AppCenter_Android_Secret;
        }

        public static class SecretsProviderConstants
        {
            public static readonly string EncryptionKeyFieldName = Configuration.Helpers.Secrets.SecretsProvider_EncryptionKeyFieldName;
            public static readonly string DeviceIdentifierFieldName = Configuration.Helpers.Secrets.SecretsProvider_DeviceIdentifierFieldName;
            public static readonly string InitializedFlagFieldName = Configuration.Helpers.Secrets.SecretsProvider_InitializedFlagFieldName;
            public static readonly string IsFirstRunFieldName = Configuration.Helpers.Secrets.SecretsProvider_IsFirstRunFieldName;
        }

        public static class PushNotificationsConstants
        {
            public static readonly string NotificationChannelName = Configuration.Helpers.Secrets.PushNotifications_NotificationChannelName;
            public static readonly string NotificationHubName = Configuration.Helpers.Secrets.PushNotifications_NotificationHubName;
            public static readonly string ListenConnectionString = Configuration.Helpers.Secrets.PushNotifications_ListenConnectionString;

            public static string[] SubscriptionTags { get; internal set; }
            public static string FCMTemplateBody { get; internal set; }
            public static string APNTemplateBody { get; internal set; }
        }
    }
}
