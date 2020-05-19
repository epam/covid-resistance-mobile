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
using Android.Content.PM;
using Android.OS;
using AndroidX.AppCompat.App;

namespace Covi.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", Icon = "@mipmap/icon",
              ScreenOrientation = ScreenOrientation.Portrait, MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //// Workaround to bug in Android.
            //// Prevents second initialization when foregrounding freshly installed app by tapping app icon.
            //// https://issuetracker.google.com/issues/36907463
            if (Intent.Flags.HasFlag(ActivityFlags.BroughtToFront))
            {
                Finish();
                return;
            }

            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.AddFlags(ActivityFlags.SingleTop);
            StartActivity(intent);
            Finish();
        }
    }
}
