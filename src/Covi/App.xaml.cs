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
using System.Threading.Tasks;
using Covi.Features;
using Covi.Features.AppSettings;
using Covi.Features.UserProfile.Services;
using Covi.Logs;
using Covi.Services.Security;
using Covi.Services.Security.SecretsProvider;
using Microsoft.Extensions.Logging;
using Prism;
using Prism.Behaviors;
using Prism.Ioc;
using Prism.Logging;
using Prism.Modularity;

namespace Covi
{
    public partial class App
    {
        private Microsoft.Extensions.Logging.ILogger _logger;

        /*
        * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
        * This imposes a limitation in which the App class must have a default constructor.
        * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
        */
        public App()
            : this(null)
        {
        }

        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            try
            {
                Xamarin.Essentials.VersionTracking.Track();

                TaskScheduler.UnobservedTaskException += (sender, ex) =>
                {
                    _logger.LogError("Exception: " + ex.ToString());
                };

#if DEBUG
                SetForDebugging(_logger);
#endif

                InitializeComponent();

                var startupRoute = this.Container.Resolve<Features.Routes.IStartupRoute>();
                await startupRoute.ExecuteAsync(this.NavigationService);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception: App initialization  failed. Reason: " + ex.ToString());
                Debugger.Break();
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            AppStateContainer.GetInstance().IsAppActive = true;
        }

        protected override void OnResume()
        {
            base.OnResume();

            AppStateContainer.GetInstance().IsAppActive = true;
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            AppStateContainer.GetInstance().IsAppActive = false;

            try
            {
                var storage = App.Current.Container.Resolve<Services.Storage.IStorageService>();
                storage?.Suspend();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database suspension failed.");
            }
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            SetupLogger(containerRegistry);

            containerRegistry.RegisterSingleton<IPageBehaviorFactory, CustomPageBehaviorFactory>();
            containerRegistry.RegisterForNavigation<Features.Shell.ShellPage>();
            containerRegistry.Register<Features.Routes.IStartupRoute, Features.Routes.StartupRoute>();

            containerRegistry.RegisterInstance<Client.Services.PlatformClientOptions>(
                (new Client.Services.PlatformClientOptions(new Uri(Configuration.Constants.PlatformConstants.EndpointUrl))));
        }

        private void SetupLogger(IContainerRegistry containerRegistry)
        {
            var loggerFactory = Logs.Logger.Factory;
            containerRegistry.RegisterInstance<Microsoft.Extensions.Logging.ILoggerFactory>(loggerFactory);
            _logger = loggerFactory.CreateLogger(nameof(Covi.App));
            containerRegistry.RegisterInstance<ILoggerFacade>(new MicrosoftExtensionsLoggingFacade(_logger));
        }

        protected override void ConfigureAggregateLogger(IAggregateLogger aggregateLogger, IContainerProvider container)
        {
            base.ConfigureAggregateLogger(aggregateLogger, container);
            aggregateLogger.AddLogger(new PrismConsoleLogger());
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            // Application base services registration
            moduleCatalog.AddModule<Services.Serialization.SerializationModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<SecurityModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Services.Dispatcher.DispatcherModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Services.Storage.StorageModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Services.Notification.NotificationModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Services.ApplicationMetadata.MetadataModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Services.Platform.PlatformModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Services.ErrorHandlers.ErrorHandlerModule>(InitializationMode.WhenAvailable);

            // Application features registrations
            moduleCatalog.AddModule<Features.BluetoothTracing.TracingEngineModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.Account.AccountModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.CreateProfile.CreateProfileModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.Main.MainModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.MedicalCodeSharing.MedicalCodeSharingModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.OnBoarding.OnBoardingModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.UserProfile.UserProfileModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.Recommendations.RecommendationsModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.UserLogIn.LogInModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.ApplyCode.ApplyCodeModule>(InitializationMode.WhenAvailable);

            moduleCatalog.AddModule<Features.Medical.MedicalModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.MedicalLogin.MedicalLoginModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<Features.MedicalChangeStatus.MedicalChangeStatusModule>(InitializationMode.WhenAvailable);

            moduleCatalog.AddModule<Features.PushNotifications.PushNotificationsModule>(InitializationMode.WhenAvailable);
            moduleCatalog.AddModule<AppSettingsModule>(InitializationMode.WhenAvailable);

            // Initialize platform specific modules
            var moduleCatalogInitializer = Container.Resolve<IModuleCatalogInitializer>();
            moduleCatalogInitializer.ConfigureModuleCatalog(moduleCatalog);
        }

        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            if (uri == Covi.Features.DeepLinks.DeepLinks.UserStatusUpdateLink)
            {
                try
                {
                    ((IUserService)Container.Resolve(typeof(IUserService)))?.GetUserStateAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update user state after push notification tapped.");
                }
            }
        }

#if DEBUG
        private void SetForDebugging(Microsoft.Extensions.Logging.ILogger logger)
        {
            Xamarin.Forms.Internals.Log.Listeners.Add(new DebugListener(logger));
        }

        private class DebugListener : Xamarin.Forms.Internals.LogListener
        {
            private readonly Microsoft.Extensions.Logging.ILogger _logger;

            public DebugListener(Microsoft.Extensions.Logging.ILogger logger)
            {
                _logger = logger;
            }

            public override void Warning(string category, string message)
            {
                _logger.LogDebug($"{category}: {message}");
            }
        }
#endif
    }
}
