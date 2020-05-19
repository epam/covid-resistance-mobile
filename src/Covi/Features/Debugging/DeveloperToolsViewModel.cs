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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Covi.Features.BluetoothTracing;
using Covi.Features.BluetoothTracing.DeviceProcessor;
using Covi.Features.BluetoothTracing.TracesStorage;
using Covi.Features.BluetoothTracing.TracingInformation;
using Covi.Services.Security.SecretsProvider;
using DynamicData;
using Prism.Navigation;
using ReactiveUI;

namespace Covi.Features.Debugging
{
    public class DeveloperToolsViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly ITracingEngine _tracingEngine;
        private ObservableCollection<TraceViewModel> _devicesCollection = new ObservableCollection<TraceViewModel>();

        public ReactiveCommand<Unit, Unit> RestartBluetoothCommand { get; }

        public ReactiveCommand<Unit, Unit> AddContactDeviceCommand { get; }

        public ReactiveCommand<Unit, Unit> RefreshContactedDevicesCommand { get; }

        public ReactiveCommand<Unit, Unit> CleanContactedDevicesStorageCommand { get; }

        public ReadOnlyObservableCollection<TraceViewModel> ContactedDevices { get; }

        public DeveloperToolsViewModel(INavigationService navigationService, Features.BluetoothTracing.ITracingEngine tracingEngine)
        {
            _navigationService = navigationService;
            _tracingEngine = tracingEngine;
            RestartBluetoothCommand = ReactiveCommand.CreateFromTask(RestartBluetoothAsync);
            AddContactDeviceCommand = ReactiveCommand.CreateFromTask(AddContactDeviceAsync);
            RefreshContactedDevicesCommand = ReactiveCommand.CreateFromTask(RefreshContactedDevicesAsync);
            CleanContactedDevicesStorageCommand = ReactiveCommand.CreateFromTask(CleanContactedDevicesStorageAsync);
            ContactedDevices = new ReadOnlyObservableCollection<TraceViewModel>(_devicesCollection);
        }

        public string DeviceId { get; private set; }

        public string ContactDeviceId { get; set; }

        public override void Initialize(INavigationParameters parameters)
        {
            base.Initialize(parameters);
            InitializeAsync().FireAndForget();
        }

        private async Task InitializeAsync()
        {
            var deviceId = await SecretsProvider.Instance.GetDeviceIdentifierAsync();
            DeviceId = deviceId;
            await RefreshContactedDevicesAsync();
        }

        private Task AddContactDeviceAsync()
        {
            var deviceId = ContactDeviceId;
            var trace = new ContactDescriptor(deviceId, DateTime.UtcNow);

            ContactDeviceId = string.Empty;

            DeviceProcessorProvider.GetInstance().Process(trace);
            return Task.CompletedTask;
        }

        private async Task RestartBluetoothAsync()
        {
            try
            {
                var deviceId = await SecretsProvider.Instance.GetDeviceIdentifierAsync();
                var tracingInformation = new TracingInformation(true, deviceId, Configuration.Constants.BluetoothConstants.ServiceUUID, Configuration.Constants.BluetoothConstants.CharacteristicUUID, Configuration.Constants.BluetoothConstants.DataExpirationTime);
                await TracingInformationContainer.Instance.SetAsync(tracingInformation);
                await _tracingEngine.RestartAsync();
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private async Task RefreshContactedDevicesAsync()
        {
            var items = await TracesStorage.Instance.QueryAsync(x => true);
            _devicesCollection.Clear();
            _devicesCollection.Add(items.Select(x => new TraceViewModel(x)));
        }

        private async Task CleanContactedDevicesStorageAsync()
        {
            await TracesStorage.Instance.Purge(DateTime.UtcNow);
            await RefreshContactedDevicesAsync();
        }

        public class TraceViewModel
        {
            public TraceViewModel(Trace trace)
            {
                ContactToken = trace.ContactToken;
                ContactTimestamp = trace.ContactTimestamp.ToLocalTime().ToString();
            }

            public string ContactTimestamp { get; set; }

            public string ContactToken { get; set; }
        }
    }
}
