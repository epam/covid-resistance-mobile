using Android.OS;
using Android.Views;
using Covi.PlatformSpecific;
using Plugin.CurrentActivity;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(Covi.Droid.CustomRenderers.Statusbar))]
namespace Covi.Droid.CustomRenderers
{
    public class Statusbar : IStatusBarPlatformSpecific
    {
        public Statusbar()
        {
        }

        public void SetStatusBarColor(Color color)
        {
            // The SetStatusBarcolor is new since API 21
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                CrossCurrentActivity.Current.Activity.Window.SetStatusBarColor(color.ToAndroid());

                var brightness = ColorConverters.FromHex(color.ToHex()).GetBrightness();
                Change(brightness >= 0.7);
            }
            else
            {
                // Here you will just have to set your
                // color in styles.xml file as shown below.
            }
        }

        private void Change(bool turnOnDarkText)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var activity = CrossCurrentActivity.Current.Activity;

                int newUiVisibility = (int)activity.Window.DecorView.SystemUiVisibility;

                if (turnOnDarkText)
                {
                    newUiVisibility |= (int)SystemUiFlags.LightStatusBar;
                }
                else
                {
                    newUiVisibility &= ~(int)SystemUiFlags.LightStatusBar;
                }

                activity.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)newUiVisibility;
            }
        }
    }
}
