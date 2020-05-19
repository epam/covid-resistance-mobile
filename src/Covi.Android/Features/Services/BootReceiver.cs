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

using Android.App;
using Android.Content;
using Android.OS;

namespace Covi.Droid.Features.Services
{
    [BroadcastReceiver(Enabled = true)]
    [IntentFilter(new[] { Intent.ActionBootCompleted, "QUICKBOOT_POWERON" })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action == Intent.ActionBootCompleted)
            {
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    JobService.EnqueueWork(context, new Intent());
                }
                else
                {
                    context.StartService(BtService.GetServiceIntent(context));
                }
            }
        }
    }
}
