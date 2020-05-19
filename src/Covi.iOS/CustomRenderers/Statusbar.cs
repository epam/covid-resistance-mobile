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

using Covi.PlatformSpecific;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(Covi.iOS.CustomRenderers.Statusbar))]

namespace Covi.iOS.CustomRenderers
{
    public class Statusbar : IStatusBarPlatformSpecific
    {
        private int _version = -1;

        public Statusbar()
        {
        }

        public void SetStatusBarColor(Color color)
        {
            if (_version == -1)
            {
                CheckVersion();
            }

            UIView statusBar;

            if (_version >= 13)
            {
                var statusBarTemp = UIApplication.SharedApplication.StatusBarFrame;
                statusBar = new UIView(statusBarTemp);
            }
            else
            {
                statusBar = UIApplication.SharedApplication.ValueForKey(
                    new NSString("statusBar")) as UIView;
            }

            if (statusBar != null && statusBar.RespondsToSelector(
            new ObjCRuntime.Selector("setBackgroundColor:")))
            {
                // change to your desired color
                statusBar.BackgroundColor = color.ToUIColor();
            }

            UIApplication.SharedApplication.KeyWindow.AddSubview(statusBar);
        }

        private void CheckVersion()
        {
            int version = 0;
            try
            {
                var ios = UIDevice.CurrentDevice.SystemVersion;
                ios = ios.Substring(0, ios.IndexOf("."));
                int.TryParse(ios, out version);
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            _version = version;
        }
    }
}
