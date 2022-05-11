using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Preference;
using System;

namespace com.companyname.NavigationGraph3.Fragments
{
    public class SettingsFragment : PreferenceFragmentCompat
    {
        private ColorThemeListPreference colorThemeListPreference;
        
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

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            SetPreferencesFromResource(Resource.Xml.preferences, rootKey);

            if (PreferenceScreen.FindPreference("darkTheme") is CheckBoxPreference checkboxDarkThemePreference)
                checkboxDarkThemePreference.PreferenceChange += CheckboxDarkThemePreference_PreferenceChange;

            colorThemeListPreference = PreferenceScreen.FindPreference("colorThemeValue") as ColorThemeListPreference;
            if (colorThemeListPreference != null)
            {
                colorThemeListPreference.Init();
                colorThemeListPreference.PreferenceChange += ColorThemeListPreference_PreferenceChange;
            }
        }

        private void CheckboxDarkThemePreference_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            // setDefaultNightMode() automatically recreates any started activities. e.g. you end up in OnCreate of MainActivity
            bool nightModeActive = (bool)e.NewValue;
            AppCompatDelegate.DefaultNightMode = nightModeActive ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;
        }

        private void ColorThemeListPreference_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            ColorThemeListPreference colorThemeListPreference = (ColorThemeListPreference)e.Preference;
            
            ISharedPreferencesEditor editor = colorThemeListPreference.SharedPreferences.Edit();
            editor.PutString("colorThemeValue", e.NewValue.ToString()).Apply();
            
            int index = Convert.ToInt16(e.NewValue.ToString());
            string colorThemeValue = colorThemeListPreference.GetEntries()[index - 1];
            colorThemeListPreference.Summary = (index != -1) ? colorThemeValue : colorThemeListPreference.DefaultThemeValue;

            // Must now force the theme to change if it changed - see BaseActivity. It's OnCreate checks the sharedPreferences, get the currentTheme and passes that value to SetAppTheme(currentTheme)
            // which checks to see if it has changed and if so calls SetTheme which the correct Resource.Style.Theme_Name)
            Activity.Recreate();
        }
    }
}