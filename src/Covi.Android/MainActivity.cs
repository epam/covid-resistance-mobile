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
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Covi.Configuration;
using Covi.Droid.Configuration;
using Covi.Features;
using Covi.Services.Storage;
using FFImageLoading.Forms.Platform;
using Microsoft.AppCenter;
using Mobile.BuildTools.Configuration;
using Plugin.CurrentActivity;
using Prism;
using Prism.Ioc;
using Prism.Modularity;

namespace Covi.Droid
{
    [Activity(Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

#if DEBUG || ANALYTICS
            if (!string.IsNullOrWhiteSpace(Constants.AppCenterConstants.Secret_Android))
            {
                AppCenter.Start(Constants.AppCenterConstants.Secret_Android,
                    typeof(Microsoft.AppCenter.Analytics.Analytics),
                    typeof(Microsoft.AppCenter.Crashes.Crashes));
                Covi.Logs.Logger.Factory.AddProvider(new Covi.Droid.Services.Log.AppCenterLogProvider());
            }
#endif

            base.OnCreate(savedInstanceState);

            ConfigurationManager.Init(true, this);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            CachedImageRenderer.Init(true);
            CachedImageRenderer.InitImageViewHandler();
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            LoadApplication(new App(new PlatformInitializer()));

            if (IsPlayServiceAvailable())
            {
                CreateNotificationChannel();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            Xamarin.Forms.Application.Current.SendOnAppLinkRequestReceived(new Uri("covi://updateUserState"));

            base.OnNewIntent(intent);
        }

        bool IsPlayServiceAvailable()
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (resultCode != ConnectionResult.Success)
            {
                if (GoogleApiAvailability.Instance.IsUserResolvableError(resultCode))
                {
                    Log.Debug("CrownResistanceDebug", GoogleApiAvailability.Instance.GetErrorString(resultCode));
                }
                else
                {
                    Log.Debug("CrownResistanceDebug", "This device is not supported");
                }
                return false;
            }
            return true;
        }

        void CreateNotificationChannel()
        {
            // Notification channels are new as of "Oreo".
            // There is no need to create a notification channel on older versions of Android.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channelName = Covi.Configuration.Constants.PushNotificationsConstants.NotificationChannelName;
                var channelDescription = string.Empty;
                var channel = new NotificationChannel(channelName, channelName, NotificationImportance.Default)
                {
                    Description = channelDescription
                };

                var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }
        }
    }


    public class PlatformInitializer : IPlatformInitializer, IModuleCatalogInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.RegisterDelegate<IConfigurationManager>(() => ConfigurationManager.Current);
            var droidConfiguration = new DroidConfiguration(ConfigurationManager.Current);
            containerRegistry.RegisterInstance<IEnvironmentConfiguration>(droidConfiguration);
            Constants.Initialize(droidConfiguration);

            containerRegistry.RegisterInstance<IModuleCatalogInitializer>(this);

            containerRegistry.Register<Covi.Services.Http.IHttpClientHandlerProvider, Services.Http.NativeHttpClientHandlerProvider>();
            containerRegistry.RegisterDelegate<Covi.Features.BluetoothTracing.IPlatformTracingEngine>(() =>
            {
                var engine = Features.Bluetooth.TracingEngine.Instance;
                return engine;
            });
        }

        public void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
#if DEBUG || ANALYTICS
            moduleCatalog.AddModule<Covi.Features.Debugging.DebuggingModule>(InitializationMode.WhenAvailable);
#endif
        }
    }
}
