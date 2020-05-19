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

using CoreAnimation;
using CoreGraphics;
using Covi.iOS.CustomRenderers;
using Covi.PlatformSpecific;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(DashedLine), typeof(DashedLineIosRenderer))]
namespace Covi.iOS.CustomRenderers
{
    public class DashedLineIosRenderer : ViewRenderer
    {
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var dashedLine = (DashedLine)Element;
            var path = new UIBezierPath();
            path.MoveTo(new CGPoint(0, 0));
            path.AddLineTo(new CGPoint(Bounds.Size.Width, 0));

            CAShapeLayer viewBorder = new CAShapeLayer
            {
                StrokeColor = dashedLine.Color.ToCGColor(),
                FillColor = null,
                LineWidth = 1,
                LineDashPattern = new NSNumber[] { new NSNumber(10), new NSNumber(10) },
                Path = path.CGPath
            };
            Layer.AddSublayer(viewBorder);
        }
    }
}
