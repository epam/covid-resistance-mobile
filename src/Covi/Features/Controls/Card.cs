#pragma warning disable SA1636 // File header copyright text should match
// https://github.com/davidortinau/Xappy/blob/b6cf4428040c2961ab60d151398aa6ae334c5ba2/Xappy/Xappy/Content/Scenarios/OtherLogin/Controls/Card.cs

using System.Linq;
#pragma warning restore SA1636 // File header copyright text should match
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Covi.Features.Controls
{
    public class Card : Frame
    {
        public static readonly BindableProperty ElevationProperty = BindableProperty.Create(
            nameof(Elevation),
            typeof(int),
            typeof(Card),
            4,
            propertyChanged: (control, _, __) => ((Card)control).UpdateElevation());

        public int Elevation
        {
            get { return (int)GetValue(ElevationProperty); }
            set { SetValue(ElevationProperty, value); }
        }

        public Card()
        {
            HasShadow = false;
            Padding = new Thickness(12, 6);
            BackgroundColor = Color.Transparent;
        }

        private void UpdateElevation()
        {
            On<iOS>()
                .SetIsShadowEnabled(true)
                .SetShadowColor(Color.Black)
                .SetShadowOffset(new Size(0, 4))
                .SetShadowOpacity(0.4)
                .SetShadowRadius(Elevation / 2);

            On<Android>()
                .SetElevation(Elevation);
        }

        /// <summary>
        /// Fix for styling a few properties (Padding, BackgroundColor...)
        /// </summary>
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == StyleProperty.PropertyName)
            {
                if (Style.Setters.Any(s => s.Property == PaddingProperty))
                {
                    Padding = (Thickness)Style.Setters.First(s => s.Property == PaddingProperty).Value;
                }

                if (Style.Setters.Any(s => s.Property == BackgroundColorProperty))
                {
                    BackgroundColor = (Color)Style.Setters.First(s => s.Property == BackgroundColorProperty).Value;
                }
            }
        }
    }
}
