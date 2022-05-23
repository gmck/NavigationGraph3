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
        private string currentTheme;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            currentTheme = sharedPreferences.GetString("colorThemeValue", "1");
            
            SetAppTheme(currentTheme);
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