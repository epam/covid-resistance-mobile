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
using System.Diagnostics;
using System.Linq;
using Covi.Features.PushNotifications;
using Covi.Features.PushNotifications.Models;
using Covi.iOS.Features.Bluetooth;
using Covi.iOS.Features.PushNotifications;
using FFImageLoading.Forms.Platform;
using Foundation;
using Microsoft.Extensions.Logging;
using Prism;
using Prism.Ioc;
using UIKit;
using WindowsAzure.Messaging;
using Covi.Features.PushNotifications.Services;
using UserNotifications;
using Covi.Configuration;
using Mobile.BuildTools.Configuration;
using Covi.iOS.Configuration;
using Covi.Features;
using Prism.Modularity;

namespace Covi.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        private ILogger _logger;

        // This method is invoked when the application has loaded and is ready to run. In this
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            ConfigurationManager.Init(true);

#if DEBUG || ANALYTICS
            if (!string.IsNullOrWhiteSpace(Constants.AppCenterConstants.Secret_iOS))
            {
                Microsoft.AppCenter.AppCenter.Start(Constants.AppCenterConstants.Secret_iOS,
                    typeof(Microsoft.AppCenter.Analytics.Analytics),
                    typeof(Microsoft.AppCenter.Crashes.Crashes));
                Covi.Logs.Logger.Factory.AddProvider(new Covi.iOS.Services.Log.AppCenterLogProvider());
            }
#endif

            global::Xamarin.Forms.Forms.Init();
            InitRenderersAndServices();

            LoadApplication(new App(new PlatformInitializer()));

            UNUserNotificationCenter.Current.Delegate = new NotificationDelegate();

            var baseResult = base.FinishedLaunching(app, options);

            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            _logger = Logs.Logger.Get(this);

            return baseResult;
        }

        private void InitRenderersAndServices()
        {
            CachedImageRenderer.Init();
            CachedImageRenderer.InitImageSourceHandler();
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            SendPushTokenToServer(deviceToken);
            SendPushTokenToAzure(deviceToken);
        }

        private void SendPushTokenToServer(NSData deviceToken)
        {
            try
            {
                var array = deviceToken?.ToArray();
                if (array?.Length > 0)
                {
                    var currentDeviceToken = BitConverter
                        .ToString(array)
                        .Replace("-", string.Empty)
                        .ToLower();

                    PushNotificationsContainer.Instance.SetAsync(new PushNotificationTokenModel { Token = currentDeviceToken, Type = "apns" });
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                _logger.LogError(ex, "Failed to send push notification token to backend");
            }
        }

        private void SendPushTokenToAzure(NSData deviceToken)
        {
            try
            {
                var hub = new SBNotificationHub(Constants.PushNotificationsConstants.ListenConnectionString, Constants.PushNotificationsConstants.NotificationHubName);

                // update registration with Azure Notification Hub
                hub.UnregisterAll(deviceToken, (error) =>
                {
                    if (error != null)
                    {
                        Debug.WriteLine($"Unable to call unregister {error}");
                        return;
                    }

                    var tags = new NSSet(Constants.PushNotificationsConstants.SubscriptionTags.ToArray());
                    hub.RegisterNative(deviceToken, tags, (errorCallback) =>
                    {
                        if (errorCallback != null)
                        {
                            Debug.WriteLine($"RegisterNativeAsync error: {errorCallback}");
                        }
                    });

                    var templateExpiration = DateTime.Now.AddDays(120)
                        .ToString(System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    hub.RegisterTemplate(deviceToken, "defaultTemplate", Constants.PushNotificationsConstants.APNTemplateBody, templateExpiration, tags,
                        (errorCallback) =>
                        {
                            if (errorCallback != null)
                            {
                                Debug.WriteLine($"RegisterTemplateAsync error: {errorCallback}");
                            }
                        });
                });
            }
            catch (Exception ex)
            {
                Debugger.Break();
                _logger.LogError(ex, "Failed to send push notification token to Azure Hub");
            }
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            Xamarin.Forms.Application.Current.SendOnAppLinkRequestReceived(new Uri("covi://updateUserState"));

            return true;
        }
    }

    public class PlatformInitializer : IPlatformInitializer, IModuleCatalogInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register any platform specific implementations
            containerRegistry.RegisterDelegate<IConfigurationManager>(() => ConfigurationManager.Current);
            var droidConfiguration = new iOSConfiguration(ConfigurationManager.Current);
            containerRegistry.RegisterInstance<IEnvironmentConfiguration>(droidConfiguration);
            Constants.Initialize(droidConfiguration);

            containerRegistry.RegisterInstance<IModuleCatalogInitializer>(this);

            containerRegistry.Register<Covi.Services.Http.IHttpClientHandlerProvider, Services.Http.NativeHttpClientHandlerProvider>();
            containerRegistry.RegisterDelegate<Covi.Features.BluetoothTracing.IPlatformTracingEngine>(() =>
            {
                return Features.Bluetooth.TracingEngine.Instance;
            });
            containerRegistry.Register<IPushNotificationInitializer, PushNotificationPermissionInitializer>();
        }

        public void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
#if DEBUG || ANALYTICS
            moduleCatalog.AddModule<Covi.Features.Debugging.DebuggingModule>(InitializationMode.WhenAvailable);
#endif
        }
    }
}
