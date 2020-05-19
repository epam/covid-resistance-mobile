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
using CoreGraphics;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Covi.iOS.Effects
{
    class BorderlessEntryEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            try
            {
                if (Control is UITextField textField)
                {
                    textField.LeftView = new UIView(new CGRect(0, 0, 0, textField.Frame.Height));
                    textField.RightView = new UIView(new CGRect(0, 0, 0, textField.Frame.Height));
                    textField.LeftViewMode = UITextFieldViewMode.Always;
                    textField.RightViewMode = UITextFieldViewMode.Always;
                    textField.BorderStyle = UITextBorderStyle.None;
                }
            }
            catch (Exception ex)
            {
                Xamarin.Forms.Internals.Log.Warning("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
        }
    }
}
