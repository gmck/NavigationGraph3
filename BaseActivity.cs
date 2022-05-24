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
        
        private bool nightModeActive;  // changed from protected
        private string currentTheme;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            currentTheme = sharedPreferences.GetString("colorThemeValue", "1");
            nightModeActive = sharedPreferences.GetBoolean("darkTheme", false);             //dark theme is active or not.

            SetAppTheme(currentTheme);

            // Removed this line from MainActivity's OnDestinationChanged and added it here. Don't think we need it anywhere other than in SettingsFragment checkboxDarkThemePreference.PreferenceChange
            // Tested we do need it here to!
            // This is best demonstrated by putting System.Threading.Thread.Sleep(2000) in the MainActivity before SetContentView. 
            AppCompatDelegate.DefaultNightMode = nightModeActive ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;
        }

        private void SetAppTheme(string currentTheme)
        {
            if (currentTheme == "1")
                SetTheme(Resource.Style.Theme_NavigationGraph_RedBmw);
            else if (currentTheme == "2")
                SetTheme(Resource.Style.Theme_NavigationGraph_BlueAudi);
            else if (currentTheme == "3")
                SetTheme(Resource.Style.Theme_NavigationGraph_GreenBmw);
        }

        protected override void OnResume()
        {
            // I don't think we even need this OnResume, as I've never seen Activity.Recreate being called from here.
            base.OnResume();

            string selectedTheme = sharedPreferences.GetString("colorThemeValue", "1");
            if (currentTheme != selectedTheme)
                Recreate();
        }
    }
}