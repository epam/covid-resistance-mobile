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
using AndroidX.Core.App;

namespace Covi.Droid.Features.Services
{
    [Service(Label = "JobService", Permission = "android.permission.BIND_JOB_SERVICE")]
    public class JobService : JobIntentService
    {
        private static readonly int JOB_ID = 1;

        public static void EnqueueWork(Context context, Intent work)
        {
            ComponentName component = new ComponentName(context, Java.Lang.Class.FromType(typeof(JobService)).Name);
            JobIntentService.EnqueueWork(context, component, JOB_ID, work);
        }

        protected override void OnHandleWork(Intent intent) =>
            BaseContext.StartForegroundService(BtService.GetServiceIntent(BaseContext));
    }
}
