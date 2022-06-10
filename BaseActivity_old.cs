using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Preference;
using System;

namespace com.companyname.NavigationGraph3
{
    [Activity(Label = "BaseActivity")]
    public class BaseActivity_old : AppCompatActivity
    {
        protected ISharedPreferences sharedPreferences;

        private bool requestedNightMode;  // changed from protected
        private string requestedColorTheme;
        private string requestedSystemTheme;

        //private UiModeManager uiModeManager;

        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);

            requestedColorTheme = sharedPreferences.GetString("colorThemeValue", "1");

            requestedNightMode = sharedPreferences.GetBoolean("darkTheme", false);

            requestedSystemTheme = sharedPreferences.GetString("systemThemeValue", "1");

            //SetDefaultNightMode(requestedNightMode);
            SetDefaultNightMode_1(requestedNightMode);
            //SetDefaultNightMode_2(requestedSystemTheme);

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

        #region SetDefaultNightMode // This looks like the one I was looking for last night
        private void SetDefaultNightMode(bool requestedNightMode)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q) //Android 10 and above
            {
                // This works via the quick settings menu on Android 10 and above, correct back ground colour for both Light and Dark.
                // However not via Settings - therefore need to disable in Settings for Android 10 and above
                int defaultNightMode = AppCompatDelegate.DefaultNightMode;
                switch (defaultNightMode)
                {
                    case AppCompatDelegate.ModeNightNo:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                        break;
                    case AppCompatDelegate.ModeNightYes:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                        break;
                    case AppCompatDelegate.ModeNightUnspecified:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightUnspecified; //defaultNightMode;  //
                        break;
                    case AppCompatDelegate.ModeNightFollowSystem:
                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;
                        break;
                }
            }
            else // Handle devices < Android 10
            {
                if (requestedNightMode)
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                else
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            }
        }
        #endregion

        #region SetDefaultNightMode_1
        private void SetDefaultNightMode_1(bool requestedNightMode)
        {
            // This works well from Settings, but only has a white background colour for both Dark and Light Modes
            bool isNightModeActive = IsNightModeActive();

            int mode = AppCompatDelegate.DefaultNightMode;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Q) //Android 10 and above
            {
                if (!requestedNightMode & !isNightModeActive)
                    return;
                else if (requestedNightMode & !isNightModeActive)
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                else if (!requestedNightMode & isNightModeActive)
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;

            }
            else
            {
                if (requestedNightMode)
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                else
                    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            }
        }
        #endregion


        #region SetDefaultNightMode_2
        private void SetDefaultNightMode_2(string currentSystemTheme )
        {
            bool isNightModeActive = IsNightModeActive();
            if (currentSystemTheme == "1" && !isNightModeActive)                             // System Default
                return;
            else if (currentSystemTheme == "1" && isNightModeActive)
                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;

            else if (currentSystemTheme == "2" && isNightModeActive)                         // Light
                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
            else if (currentSystemTheme == "2" && !isNightModeActive)
                return;

            else if (currentSystemTheme == "3" && !isNightModeActive)                        // Dark
                AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
            else if (currentSystemTheme == "3" && isNightModeActive)
                return;
        }
        #endregion

        

        #region OnResume
        protected override void OnResume()
        {
            // I don't think we even need this OnResume, as I've never seen Activity.Recreate being called from here.
            base.OnResume();

            string selectedTheme = sharedPreferences.GetString("colorThemeValue", "1");
            if (requestedColorTheme != selectedTheme)
                Recreate();
        }
        #endregion

        #region IsNightModeActive
        private bool IsNightModeActive()
        {
            // Notes

            // ModeNightUnspecified     = -100
            // ModeNightFollowSystem    = -1
            // ModeNightNo              = 1
            // ModeNightYes             = 2
            
            // UiMode.Undefined = 0
            // UiMode.NightNo   = 0x10
            // UiMode.NightYes  = 0x20
            // UiMode.NightMask = 48

            //int defaultNightMode = AppCompatDelegate.DefaultNightMode;

            //if (defaultNightMode == AppCompatDelegate.ModeNightYes)
            //    return true;
            //if (defaultNightMode == AppCompatDelegate.ModeNightNo)
            //    return false;

            return (int)(Resources.Configuration.UiMode & UiMode.NightMask) switch
            {
                (int)UiMode.NightYes => true,
                (int)UiMode.NightNo => false,
                (int)UiMode.NightUndefined => false,
                _ => false,
            };
        }
        #endregion


        

        #region GetRequestSystemTheme
        private AppCompatDelegate GetRequestedSystemTheme(string currentSystemTheme)
        {
            if (currentSystemTheme == "1")
                return (AppCompatDelegate)AppCompatDelegate.ModeNightUnspecified;
            else if (currentSystemTheme == "2")
                return (AppCompatDelegate)AppCompatDelegate.ModeNightNo;
            else if (currentSystemTheme == "3")
                return (AppCompatDelegate)AppCompatDelegate.ModeNightYes;
            else
                return (AppCompatDelegate)AppCompatDelegate.ModeNightFollowSystem;
        }
        #endregion
    }
}
//when(resources.configuration.uiMode and Configuration.UI_MODE_NIGHT_MASK)
//{
//    Configuration.UI_MODE_NIGHT_YES->
//        AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO)
//        Configuration.UI_MODE_NIGHT_NO->
//            AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES)



//int existingMode = AppCompatDelegate.DefaultNightMode;

//if (nightModeActive)
//    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
//else if (existingMode == AppCompatDelegate.ModeNightFollowSystem)
//    AppCompatDelegate.DefaultNightMode = existingMode;
//else
//    AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;



//mode = AppCompatDelegate.getDefaultNightMode();
//SharedPreferences preferencesnight = android.preference.PreferenceManager.getDefaultSharedPreferences(MyApplication.this);

//if (preferencesnight.getBoolean(KEY_SIGNATURE, false))
//  AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES);
//else if (mode == AppCompatDelegate.MODE_NIGHT_FOLLOW_SYSTEM)
//  AppCompatDelegate.setDefaultNightMode(mode);
//else
//  AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_NO);
  
