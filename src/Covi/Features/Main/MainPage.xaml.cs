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

using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace Covi.Features.Main
{
    public partial class MainPage : Xamarin.Forms.TabbedPage, INavigationPageOptions
    {
        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);
            if (Features.Debugging.DebuggingFlag.Enabled)
            {
                this.Children.Add(new Features.Debugging.DeveloperToolsPage());
            }
        }

        public bool ClearNavigationStackOnNavigation => true;
    }

    public static class StylesExtensions
    {
        public static Color LookupColor(this ResourceDictionary dictionary, string key)
        {
            try
            {
                dictionary.TryGetValue(key, out var newColor);
                return (Color)newColor;
            }
            catch
            {
                return Color.Transparent;
            }
        }
    }
}
