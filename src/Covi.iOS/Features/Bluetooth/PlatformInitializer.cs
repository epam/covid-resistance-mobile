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
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreBluetooth;
using Covi.Features.BluetoothTracing;
using UIKit;
using Xamarin.Essentials;

namespace Covi.iOS.Features.Bluetooth
{
    public class PlatformInitializer : Covi.Features.BluetoothTracing.IPlatformTracingEngineInitializer
    {
        public PlatformInitializer()
        {
        }

        public async Task<InitializationResult> InitializeAsync()
        {
            var status = await Xamarin.Essentials.Permissions.CheckStatusAsync<BluetoothCentralPermissions>();
            if (status != PermissionStatus.Granted)
            {
                status = await Xamarin.Essentials.Permissions.RequestAsync<BluetoothCentralPermissions>();
                if (status != PermissionStatus.Granted)
                {
                    return new InitializationResult()
                    {
                        PermissionStatus = status,
                        Success = status == PermissionStatus.Granted
                    };
                }
            }

            var peripheralStatus = await Xamarin.Essentials.Permissions.CheckStatusAsync<BluetoothPeripheralPermissions>();
            if (peripheralStatus != PermissionStatus.Granted)
            {
                peripheralStatus = await Xamarin.Essentials.Permissions.RequestAsync<BluetoothPeripheralPermissions>();
                return new InitializationResult()
                {
                    PermissionStatus = peripheralStatus,
                    Success = status == PermissionStatus.Granted
                };
            }

            var result = new InitializationResult()
            {
                PermissionStatus = peripheralStatus,
                Success = status == PermissionStatus.Granted
            };

            return result;
        }
    }

    public class BluetoothCentralPermissions : Xamarin.Essentials.Permissions.BasePlatformPermission
    {
        public static TimeSpan PermissionTimeout { get; set; } = TimeSpan.FromSeconds(10);

        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetBluetoothStatus());
        }

        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys => () => new string[]
        {
            "NSBluetoothAlwaysUsageDescription"
        };

        public override async Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();
            var status = GetBluetoothStatus();

            if (status == PermissionStatus.Granted || status == PermissionStatus.Disabled)
                return status;

            EnsureMainThread();

            return await RequestBluetoothAsync();
        }

        private void EnsureMainThread()
        {
            if (!MainThread.IsMainThread)
                throw new PermissionException("Permission request must be invoked on main thread.");
        }

        private static PermissionStatus GetBluetoothStatus()
        {
            PermissionStatus result = PermissionStatus.Unknown;

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                switch (CBCentralManager.Authorization)
                {
                    case CBManagerAuthorization.AllowedAlways:
                        result = PermissionStatus.Granted;
                        break;
                    case CBManagerAuthorization.Denied:
                        result = PermissionStatus.Denied;
                        break;
                    case CBManagerAuthorization.Restricted:
                        result = PermissionStatus.Restricted;
                        break;
                    default:
                        result = PermissionStatus.Unknown;
                        break;
                }
            }
            else
            {
                // Prior iOS 13 permissions were granted automatically
                result = PermissionStatus.Unknown;
            }
            return result;
        }

        private Task<PermissionStatus> RequestBluetoothAsync()
        {
            var manager = new CBCentralManager();
            var tcs = new TaskCompletionSource<PermissionStatus>(manager);
            manager.UpdatedState += StatusCollback;

            return tcs.Task.ReturnInTimeoutAsync(PermissionStatus.Unknown, PermissionTimeout);

            void StatusCollback(object sender, EventArgs e)
            {
                var state = manager.State;
                manager.UpdatedState -= StatusCollback;

                var result = PermissionStatus.Unknown;
                switch (state)
                {
                    case CBCentralManagerState.PoweredOff:
                    case CBCentralManagerState.Unsupported:
                        result = PermissionStatus.Disabled;
                        break;
                    case CBCentralManagerState.PoweredOn:
                        result = PermissionStatus.Granted;
                        break;
                    case CBCentralManagerState.Unauthorized:
                        result = PermissionStatus.Denied;
                        break;
                    case CBCentralManagerState.Resetting:
                    default:
                        result = PermissionStatus.Unknown;
                        break;
                }
                manager.Dispose();
                manager = null;
                tcs.SetResult(result);
            }
        }
    }

    public class BluetoothPeripheralPermissions : Xamarin.Essentials.Permissions.BasePlatformPermission
    {
        public static TimeSpan PermissionTimeout { get; set; } = TimeSpan.FromMinutes(1);

        public override Task<PermissionStatus> CheckStatusAsync()
        {
            EnsureDeclared();

            return Task.FromResult(GetBluetoothStatus());
        }

        protected override Func<IEnumerable<string>> RequiredInfoPlistKeys => () => new string[]
        {
            "NSBluetoothPeripheralUsageDescription"
        };

        public override async Task<PermissionStatus> RequestAsync()
        {
            EnsureDeclared();
            var status = GetBluetoothStatus();

            if (status == PermissionStatus.Granted || status == PermissionStatus.Disabled)
                return status;

            EnsureMainThread();

            return await RequestBluetoothAsync();
        }

        private void EnsureMainThread()
        {
            if (!MainThread.IsMainThread)
                throw new PermissionException("Permission request must be invoked on main thread.");
        }

        private static PermissionStatus GetBluetoothStatus()
        {
            PermissionStatus result = PermissionStatus.Unknown;

            if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            {
                switch (CBPeripheralManager.Authorization)
                {
                    case CBManagerAuthorization.AllowedAlways:
                        result = PermissionStatus.Granted;
                        break;
                    case CBManagerAuthorization.Denied:
                        result = PermissionStatus.Denied;
                        break;
                    case CBManagerAuthorization.Restricted:
                        result = PermissionStatus.Restricted;
                        break;
                    default:
                        result = PermissionStatus.Unknown;
                        break;
                }
            }
            else
            {
                switch (CBPeripheralManager.AuthorizationStatus)
                {
                    case CBPeripheralManagerAuthorizationStatus.Authorized:
                        result = PermissionStatus.Granted;
                        break;
                    case CBPeripheralManagerAuthorizationStatus.Denied:
                    case CBPeripheralManagerAuthorizationStatus.NotDetermined:
                        result = PermissionStatus.Denied;
                        break;
                    case CBPeripheralManagerAuthorizationStatus.Restricted:
                        result = PermissionStatus.Restricted;
                        break;
                    default:
                        result = PermissionStatus.Unknown;
                        break;
                }
            }
            return result;
        }

        private Task<PermissionStatus> RequestBluetoothAsync()
        {
            var manager = new CBPeripheralManager();
            var tcs = new TaskCompletionSource<PermissionStatus>(manager);
            manager.StateUpdated += StatusCollback;

            return tcs.Task.ReturnInTimeoutAsync(PermissionStatus.Unknown, PermissionTimeout);

            void StatusCollback(object sender, EventArgs e)
            {
                var state = manager.State;
                manager.StateUpdated -= StatusCollback;

                var result = PermissionStatus.Unknown;
                switch (state)
                {
                    case CBPeripheralManagerState.PoweredOff:
                    case CBPeripheralManagerState.Unsupported:
                        result = PermissionStatus.Disabled;
                        break;
                    case CBPeripheralManagerState.PoweredOn:
                        result = PermissionStatus.Granted;
                        break;
                    case CBPeripheralManagerState.Unauthorized:
                        result = PermissionStatus.Denied;
                        break;
                    case CBPeripheralManagerState.Resetting:
                    default:
                        result = PermissionStatus.Unknown;
                        break;
                }

                switch (CBPeripheralManager.AuthorizationStatus)
                {
                    case CBPeripheralManagerAuthorizationStatus.Denied:
                    case CBPeripheralManagerAuthorizationStatus.NotDetermined:
                        result = PermissionStatus.Denied;
                        break;
                    case CBPeripheralManagerAuthorizationStatus.Restricted:
                        result = PermissionStatus.Restricted;
                        break;
                }
                manager.Dispose();
                manager = null;
                tcs.SetResult(result);
            }
        }
    }
}
