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

using Android.Content;
using Android.Graphics;
using Covi.Droid.CustomRenderers;
using Covi.PlatformSpecific;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(DashedLine), typeof(DashedLineAndroidRenderer))]

namespace Covi.Droid.CustomRenderers
{
    public class DashedLineAndroidRenderer : ViewRenderer
    {
        private Paint _paint;
        private Path _path;
        private PathEffect _effects;

        public DashedLineAndroidRenderer(Context context)
            : base(context)
        {
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            var dashedLine = (DashedLine) Element;

            _paint = new Paint();
            _paint.SetStyle(Paint.Style.Stroke);
            _paint.StrokeWidth = dashedLine.LineWidth;
            _paint.Color = dashedLine.Color.ToAndroid();

            _effects = new DashPathEffect(new float[] {20, 20, 20, 20}, 0);
            _paint.SetPathEffect(_effects);

            _path = new Path();
            _path.MoveTo(0, 0);
            _path.LineTo(this.MeasuredWidth, 0);
            canvas.DrawPath(_path, _paint);
        }
    }
}
