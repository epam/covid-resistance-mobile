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
using Android.App;
using Android.Content;
using Android.OS;
using Covi.Droid.Features.Bluetooth.Advertising;
using Covi.Droid.Features.Bluetooth.Gatt;
using Covi.Droid.Features.Bluetooth;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.TracingInformation;
using Microsoft.Extensions.Logging;

namespace Covi.Droid.Features.Services
{
    [Service(Label = "BtService", Enabled = true, Exported = false)]
    public class BtService : Service
    {
        private const int NotificationId = 332;
        private const string ChannelId = "com.epam.covi";

        private static WeakReference<BtService> _serviceRef;

        public static BtService Instance
        {
            get
            {
                BtService result = null;
                _serviceRef?.TryGetTarget(out result);
                return result;
            }
        }

        public static Intent GetServiceIntent(Context context)
        {
            return new Intent(context, typeof(BtService));
        }

        private BtAdvertising _btAdvertiser;
        private BtGattServer _btGattServer;
        private BtScanner _btScanner;
        public bool Initialized { get; private set; }
        public bool Running { get; private set; }

        private ILogger _logger;

        public override StartCommandResult OnStartCommand(Android.Content.Intent intent, StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            Initialized = true;
            Running = false;
            _logger = Covi.Logs.Logger.Get(this);
            _serviceRef = new WeakReference<BtService>(this);
            base.OnCreate();
            GoForeground();
            InitBtOperations();
            Start();
            _logger.LogDebug("Bluetooth background service created.");
        }

        internal void Start()
        {
            StartBtOperations();
        }

        internal void Stop()
        {
            StopBtOperations();
        }

        public override void OnDestroy()
        {
            _logger.LogDebug("Bluetooth background service destroyed.");
            Initialized = false;
            Running = false;
            StopBtOperations();
            StopForeground(true);
            base.OnDestroy();
        }


        private void GoForeground()
        {
            Notification.Builder notificationBuilder = null;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannel();
                notificationBuilder = new Notification.Builder(this, ChannelId);
            }
            else
            {
#pragma warning disable CS0618 // Type or member is obsolete, but we support older version
                notificationBuilder = new Notification.Builder(this);
#pragma warning restore CS0618 // Type or member is obsolete
            }

            Notification notification = notificationBuilder.SetContentTitle(Covi.Resources.Localization.Android_BluetoothNotification_Title)
                    .SetContentText(Covi.Resources.Localization.Android_BluetoothNotification_Content)
                    .SetSmallIcon(Resource.Drawable.notification_icon)
                    .SetContentIntent(BuildIntentToShowMainActivity())
                    .SetOngoing(true)
                    .Build();
            StartForeground(NotificationId, notification);
            _logger.LogDebug("Bluetooth background service set to foreground.");
        }

        private PendingIntent BuildIntentToShowMainActivity()
        {
            Intent notificationIntent = new Intent(this, typeof(MainActivity));
            notificationIntent.AddFlags(ActivityFlags.ClearTop);
            notificationIntent.AddFlags(ActivityFlags.SingleTop);
            PendingIntent pendingIntent = PendingIntent.GetActivity(
                        this, 0,
                        notificationIntent, 0
                    );
            return pendingIntent;
        }

        private void CreateNotificationChannel()
        {
            var channel = new NotificationChannel(ChannelId, Covi.Resources.Localization.Android_BluetoothNotification_ChannelName, NotificationImportance.High);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        private void InitBtOperations()
        {
            _btAdvertiser = BtAdvertising.CreateAdvertiser();
            _btAdvertiser.Init();
            _btScanner = new BtScanner();
            _btScanner.Init();
            _btGattServer = new BtGattServer();
            _btGattServer.Init(this.BaseContext);
        }

        private async void StartBtOperations()
        {
            _logger.LogDebug("Bluetooth background starting operations.");

            var tracingInformation = await TracingInformationContainer.Instance.GetAsync();
            _btAdvertiser.StartAdvertising(tracingInformation);
            _btScanner.StartScanning(this.BaseContext, tracingInformation);
            _btGattServer.Start(this.BaseContext, tracingInformation);
            Running = true;
        }

        private void StopBtOperations()
        {
            _logger.LogDebug("Bluetooth background stoppingoperations.");
            _btAdvertiser.StopAdvertising();
            _btScanner.StopScanning();
            _btGattServer.Stop();
        }
    }
}
