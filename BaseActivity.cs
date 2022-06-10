using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Preference;

namespace com.companyname.NavigationGraph3
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity : AppCompatActivity
    {
        protected ISharedPreferences sharedPreferences;

        private string requestedColorTheme;
        private bool requestedNightMode;

        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);

            // colorThemeValue defaults to RedBmw
            requestedColorTheme = sharedPreferences.GetString("colorThemeValue", "1");
            requestedNightMode = sharedPreferences.GetBoolean("darkTheme", false);

            // Only for devices less than Android 10 - darkTheme defaults to false - therefore Light by default
            SetDefaultNightMode(requestedNightMode);

            SetAppTheme(requestedColorTheme);

        }
        #endregion

        #region SetAppTheme
        private void SetAppTheme(string requestedColorTheme)
        {
            if (requestedColorTheme == "1")
                SetTheme(Resource.Style.Theme_NavigationGraph_RedBmw);
            else if (requestedColorTheme == "2")
                SetTheme(Resource.Style.Theme_NavigationGraph_BlueAudi);
            else if (requestedColorTheme == "3")
                SetTheme(Resource.Style.Theme_NavigationGraph_GreenBmw);
        }
        #endregion

        #region SetDefaultNightMode
        private void SetDefaultNightMode(bool requestedNightMode)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
                AppCompatDelegate.DefaultNightMode = requestedNightMode ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;
        }
        #endregion

        #region SetDefaultNightMode // Looked like it may have been useful but wasn't required
        //private void SetDefaultNightMode(bool requestedNightMode)
        //{
        //    // If we just rely on setting light/dark via quick settings we don't even need to call this, because it is already set.

        //    // Whenever I query AppCompatDelegate.DefaultNightMode it always returns - 100 i.e. AppCompatDelegate.ModeNightUnspecified
        //    bool isNightModeActive = IsNightModeActive();

        //    if (Build.VERSION.SdkInt >= BuildVersionCodes.Q) //Android 10 and above
        //    {
        //        // This works via the quick settings menu on Android 10 and above, correct back ground colour for both Light and Dark.
        //        // However not via Settings - therefore need to disable in Settings for Android 10 and above and allow manual setting setting for < Android 10 
        //        // The other problem is that it doesn't use the application icon on Devices less than Android 10, but defaults to the Android icon
        //        // unless we supply a windowSplashScreenAnimatedIcon. Also always a white background 

        //        int mode = AppCompatDelegate.DefaultNightMode;
        //        switch (mode)
        //        {
        //            case AppCompatDelegate.ModeNightNo:
        //                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
        //                break;
        //            case AppCompatDelegate.ModeNightYes:
        //                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
        //                break;
        //            case AppCompatDelegate.ModeNightUnspecified:
        //                AppCompatDelegate.DefaultNightMode = mode; //AppCompatDelegate.ModeNightUnspecified; //mode;  //
        //                break;
        //            case AppCompatDelegate.ModeNightFollowSystem:
        //                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    else // Handle devices < Android 10
        //    {
        //        if (requestedNightMode)
        //            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
        //        else
        //            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
        //    }
        //}
        #endregion

        #region IsNightModeActive // Possible useful function that wasn't required in this project
        //private bool IsNightModeActive()
        //{
        //    // Possible useful function that wasn't required in this project

        //    // Notes

        //    // ModeNightAuto = 0        deprecated
        //    // ModeNightAutoTime = 0    deprecated

        //    // ModeNightUnspecified     = -100
        //    // ModeNightFollowSystem    = -1
        //    // ModeNightNo              = 1
        //    // ModeNightYes             = 2

        //    // UiMode.Undefined = 0
        //    // UiMode.NightNo   = 0x10
        //    // UiMode.NightYes  = 0x20
        //    // UiMode.NightMask = 48

        //    return (int)(Resources.Configuration.UiMode & UiMode.NightMask) switch
        //    {
        //        (int)UiMode.NightYes => true,
        //        (int)UiMode.NightNo => false,
        //        (int)UiMode.NightUndefined => false,
        //        _ => false,
        //    };
        //}
        #endregion

    }
}
