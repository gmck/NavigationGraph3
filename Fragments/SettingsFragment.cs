using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.Preference;
using System;

namespace com.companyname.NavigationGraph3.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        private ColorThemeListPreference colorThemeListPreference;
        private ISharedPreferences sharedPreferences;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HasOptionsMenu = true;
        }
        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            menu.Clear();
        }

        #region OnCreatePreferences
        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Activity);

            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);
            
            if (PreferenceScreen.FindPreference("darkTheme") is CheckBoxPreference checkboxDarkThemePreference)
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
                    checkboxDarkThemePreference.PreferenceChange += CheckboxDarkThemePreference_PreferenceChange;
                else
                    checkboxDarkThemePreference.Enabled = false;
            }

            if (PreferenceScreen.FindPreference("colorThemeValue") is ColorThemeListPreference colorThemeListPreference)
            {
                colorThemeListPreference.Init();
                colorThemeListPreference.PreferenceChange += ColorThemeListPreference_PreferenceChange;
            }

        }
        #endregion


        #region CheckboxDarkThemePreference_PreferenceChange
        private void CheckboxDarkThemePreference_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            ISharedPreferencesEditor editor = sharedPreferences.Edit();
            editor.PutBoolean("darkTheme", (bool)e.NewValue).Apply();
            editor.Commit();
            // This is only available to devices running less than Android 10.
            // Must now force the light or dark theme to change - see BaseActivity. It's OnCreate checks the sharedPreferences, then calls SetDefaultNightMode().
            
            Activity.Recreate();
        }
        #endregion

        #region ColorThemeListPreference_PreferenceChange
        private void ColorThemeListPreference_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            colorThemeListPreference = e.Preference as ColorThemeListPreference;

            ISharedPreferencesEditor editor = colorThemeListPreference.SharedPreferences.Edit();
            editor.PutString("colorThemeValue", e.NewValue.ToString()).Apply();

            int index = Convert.ToInt16(e.NewValue.ToString());
            string colorThemeValue = colorThemeListPreference.GetEntries()[index - 1];
            colorThemeListPreference.Summary = (index != -1) ? colorThemeValue : colorThemeListPreference.DefaultThemeValue;

            // Must now force the theme to change - see BaseActivity. It's OnCreate checks the sharedPreferences, get the string currentTheme and passes that value to SetAppTheme(currentTheme)
            // which checks to see if it has changed and if so calls SetTheme which the correct Resource.Style.Theme_Name)
            Activity.Recreate();
        }
        #endregion

    }
}