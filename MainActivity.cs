using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using AndroidX.DrawerLayout.Widget;
using AndroidX.Navigation;
using AndroidX.Navigation.Fragment;
using AndroidX.Navigation.UI;
using AndroidX.Preference;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Navigation;

namespace com.companyname.NavigationGraph3
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.NavigationGraph.RedBmw", MainLauncher = true)]
    public class MainActivity : BaseActivity, NavController.IOnDestinationChangedListener, NavigationView.IOnNavigationItemSelectedListener, IOnApplyWindowInsetsListener
    {
        internal readonly string logTag = "GLM - MainActivity";

        private AppBarConfiguration appBarConfiguration;
        private NavigationView navigationView;
        private DrawerLayout drawerLayout;
        private BottomNavigationView bottomNavigationView;
        private NavController navController;
        private Toolbar toolbar;


        // Preferences variables - see OnDestinationChanged where it is checked
        private bool devicesWithNotchesAllowFullScreen;             // allow full screen for devices with notches
        private bool nightModeActive;                               // dark theme is active or not.
        private bool animateFragments;                              // animate fragments 


        // A couple of pertinent articles from two of Google's Android Developers re WindowInsets
        // Chris Banes
        //https://medium.com/androiddevelopers/windowinsets-listeners-to-layouts-8f9ccc8fa4d1
        // Ian Lake
        //https://medium.com/androiddevelopers/why-would-i-want-to-fitssystemwindows-4e26d9ce1eec

        // Theming - Material for Android
        //https://material.io/develop/android
        
        // Theming articles. Android styling: themes vs styles - Nick Butcher
        //https://medium.com/androiddevelopers/android-styling-themes-vs-styles-ebe05f917578

        // Android Styling: prefer theme attributes - Nick Butcher
        //https://medium.com/androiddevelopers/android-styling-prefer-theme-attributes-412caa748774

        // Theming articles - Nick Rout
        //https://medium.com/androiddevelopers/migrating-to-material-components-for-android-ec6757795351

        // Material Theming with MDC - Nick Rout
        //https://medium.com/androiddevelopers/material-theming-with-mdc-color-860dbba8ce2f

        // Theming articles - Shivam Dhuria
        //https://proandroiddev.com/a-quick-guide-for-using-material-components-in-android-7d8783b7fb08

        // Dark Theme - Chris Banes - Dark Theme with MDC
        //https://medium.com/androiddevelopers/dark-theme-with-mdc-4c6fc357d956

        // Android Design System and Theming: Colors. Hugo Matilla
        //https://www.hugomatilla.com/blog/android-design-system-and-theming-colors/


        #region OnCreate
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            // This rather than android:windowTranslucentStatus in styles seems to have fixed the problem with the OK button on the BasicDialogFragment
            // It also fixes the AppBarlayout so it extends full screen, when devicesWithNotchesAllowFullScreen = true; 
            Window.AddFlags(WindowManagerFlags.TranslucentStatus);

            // Require a toolbar
            toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            ViewCompat.SetOnApplyWindowInsetsListener(toolbar, this);


            // navigationView, bottomNavigationView for NavigationUI and drawerLayout for the AppBarConfiguration and NavigationUI
            navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_nav);

            // NavHostFragment so we can get a NavController 
            NavHostFragment navHostFragment = SupportFragmentManager.FindFragmentById(Resource.Id.nav_host) as NavHostFragment;
            navController = navHostFragment.NavController;

            // These are the fragments that you don't wont the back button of the toolbar to display on e.g. topLevel fragments. They correspond to the items of the NavigationView.
            int[] topLevelDestinationIds = new int[] { Resource.Id.home_fragment, Resource.Id.gallery_fragment, Resource.Id.slideshow_fragment };
            appBarConfiguration = new AppBarConfiguration.Builder(topLevelDestinationIds).SetOpenableLayout(drawerLayout).Build();  // SetDrawerLayout replaced with SetOpenableLayout

            NavigationUI.SetupActionBarWithNavController(this, navController, appBarConfiguration);
            NavigationUI.SetupWithNavController(bottomNavigationView, navController);

            navigationView.SetNavigationItemSelectedListener(this);

            // Add the DestinationChanged listener
            navController.AddOnDestinationChangedListener(this);
        }
        #endregion

        #region OnApplyWindowInsets
        public WindowInsetsCompat OnApplyWindowInsets(View v, WindowInsetsCompat insets)
        {
            if (v is Toolbar)
            {
                AndroidX.Core.Graphics.Insets statusBarInsets = insets.GetInsets(WindowInsetsCompat.Type.StatusBars());

                SetMargins(v, statusBarInsets);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                {
                    if (insets.DisplayCutout != null)
                    {
                        if (devicesWithNotchesAllowFullScreen)
                            Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.ShortEdges;
                        else
                            Window.Attributes.LayoutInDisplayCutoutMode = LayoutInDisplayCutoutMode.Default;
                    }
                }
            }
            return insets;
        }
        #endregion

        #region SetMargins
        private void SetMargins(View v, AndroidX.Core.Graphics.Insets insets)
        {
            ViewGroup.MarginLayoutParams marginLayoutParams = (ViewGroup.MarginLayoutParams)v.LayoutParameters;
            marginLayoutParams.LeftMargin = insets.Left;
            marginLayoutParams.TopMargin = insets.Top;          // top is all we are concerned with
            marginLayoutParams.RightMargin = insets.Right;
            marginLayoutParams.BottomMargin = insets.Bottom;
            v.LayoutParameters = marginLayoutParams;
            v.RequestLayout();
        }
        #endregion

        #region OnCreateOptionsMenu
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            base.OnCreateOptionsMenu(menu);
            MenuInflater.Inflate(Resource.Menu.main, menu);
            return true;
        }
        #endregion

        #region OnSupportNavigationUp
        public override bool OnSupportNavigateUp()
        {
            // Prevent standard behavior if the fragment which has an up button (left arrow) and you don't want to go back to the home fragment.
            // See OnDestinationChanged for an alternative method
            
            //if (navController.CurrentDestination.Id == Resource.Id.leaderboard || navController.CurrentDestination.Id == Resource.Id.register)
            //{
            //    navController.PopBackStack(Resource.Id.nav_slideshow, false);
            //    navController.Navigate(Resource.Id.nav_slideshow/*, null, navOptions*/);
            //    return true;
            //}

            return NavigationUI.NavigateUp(navController, appBarConfiguration) || base.OnSupportNavigateUp();
        }
        #endregion

        #region OnOptionsItemSelected
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_settings:
                    navController.Navigate(Resource.Id.settingsFragment);
                    break;

                case Resource.Id.action_subscription_info:
                    ShowSubscriptionInfoDialog(GetString(Resource.String.subscription_explanation_title), GetString(Resource.String.subscription_explanation_text));
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        #endregion

        #region OnNavigationItemSelected
        public bool OnNavigationItemSelected(IMenuItem menuItem)
        {
            // Using Fader2 as the default as animateFragment is false by default - check AnimationResource.cs for different animations
            if (!animateFragments)
                AnimationResource.Fader2();
            else
                AnimationResource.Slider();

            NavOptions navOptions = new NavOptions.Builder()
                    .SetLaunchSingleTop(true)
                    .SetEnterAnim(AnimationResource.EnterAnimation)
                    .SetExitAnim(AnimationResource.ExitAnimation)
                    .SetPopEnterAnim(AnimationResource.PopEnterAnimation)
                    .SetPopExitAnim(AnimationResource.PopExitAnimation)
                    .Build();

            bool proceed = false;

            switch (menuItem.ItemId)
            {
                // These are all topLevel fragments
                // Add fragment classes and fragment layouts as we add to the codebase as per the NavigationView items. 
                // If any classes and layouts are missing, then the NavigationView will not update the item selected.
                // The menuitem highlight will stay on the current item and the current fragment will remain displayed, nor will the app crash.
                case Resource.Id.home_fragment:
                case Resource.Id.gallery_fragment:
                case Resource.Id.slideshow_fragment:
                    proceed = true;
                    break;

                default:
                    break;
            }
            // We have the option here of animating our toplevel destinations. If we don't want animation comment out the NavOptions or just rely on NavigationUI.OnNavDestinationSelected.
            bool handled = false;
            if (proceed)
            {
                navController.Navigate(menuItem.ItemId, null, navOptions);
                handled = true;
            }

            if (!handled)
                //Default behavior as if we didn't use OnNavigationItemSelected
                handled = NavigationUI.OnNavDestinationSelected(menuItem, Navigation.FindNavController(this, Resource.Id.nav_host)) || base.OnOptionsItemSelected(menuItem);

            if (drawerLayout.IsDrawerOpen(GravityCompat.Start))
                drawerLayout.CloseDrawer(GravityCompat.Start);

            return handled;

        }
        #endregion

        #region OnDestinationChanged
        public void OnDestinationChanged(NavController navController, NavDestination navDestination, Bundle bundle)
        {

            CheckForPreferenceChanges();

            // The first menu item is not checked by default, so we need to check it to show it is selected on the startDestination fragment.
            navigationView.Menu.FindItem(Resource.Id.home_fragment).SetChecked(navDestination.Id == Resource.Id.home_fragment);

            if (navDestination.Id == Resource.Id.home_fragment)
                AppCompatDelegate.DefaultNightMode = nightModeActive ? AppCompatDelegate.ModeNightYes : AppCompatDelegate.ModeNightNo;

            if (navDestination.Id == Resource.Id.slideshow_fragment)
            {
                bottomNavigationView.Visibility = ViewStates.Visible;
                navigationView.Visibility = ViewStates.Gone;
                drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeLockedClosed);
            }
            else
            {
                bottomNavigationView.Visibility = ViewStates.Gone;
                navigationView.Visibility = ViewStates.Visible;
                drawerLayout.SetDrawerLockMode(DrawerLayout.LockModeUnlocked);
            }

            // By default because the LeaderboardFragment and the RegisterFragment are not top level fragments, they will default to showing a up button (left arrow) plus the title.
            // If you don't want the up button, remove it here. This also means that the additional code in OnSupportNavigationUp can be removed. 
            if (navDestination.Id == Resource.Id.leaderboard_fragment || navDestination.Id == Resource.Id.register_fragment)
            {
                toolbar.Title = navDestination.Label;
                toolbar.NavigationIcon = null;
            }

            #region Notes about Window.Attributes.LayoutInDisplayCutoutMode
            // Here is a bit of a trick. If we haven't set <item name="android:windowLayoutInDisplayCutoutMode">shortEdges</item> in our theme (for whatever reason). Then OnApplyWindowInsets
            // will never be called if our DrawerLayout and NavigationView have android:fitsSystemWindows="true". 

            // Therefore to guarantee that it does get called we set it here, because we don't want to letterbox our normal layouts, especially all our landscape views with the gauge views
            // Note if you do set it in styles then it should be in values-v28 or even values-v27. Android Studio gives you a warning if you try and set it in values. The problem setting in values-28 is that values-v28
            // requires the theme of the activity. Normally our theme is the splash theme and we swap it by calling SetTheme(Resource.Style.OBDTheme) in the first line of OnCreate in the MainActivity.

            // Note: Only when devicesWithNotchesAllowFullScreen is true and therefore LayoutInDisplayCutoutMode is ShortEdges will insets.DisplayCutout not be null.
            // Whenever LayoutInDisplayCutoutMode it is default or never insets.DisplayCutout will always be null.
            // So even if a device has a notch, if devicesWithNotchesAllOwFullScreen is false then will always get Default because DisplayCutout will be null.

            // Do we need this? We only need shortEdges if we have a notch, therefore why not wait until the test in OnApplyWindowInsets?
            // Answer: We do need it here, because if ShortEdges is not set here, then later in the test in OnApplyWindowInsets, insets.DisplayCutout will be null which will always result in Default being set,
            // so we can't avoid this.
            // It is really the same as if we had set ShortEdges in styles.xml of values-v28, (which we don't want to do because we are using OBDTheme.Splash). By presetting Window.Attributes.LayoutInDisplayCutoutMode
            // here, when the user tells us they want it allows insets.DisplayCutout to be not null by the time we do the test in OnApplyWindowInsets.
            // Note the Setting in Preferences has no effect if the device does not have a notch, so no harm is done if a user accidently sets devicesWithNotchesAllowFullscreen to true.
            // TODO: Make a note in our user Guide.
            #endregion


            if (Build.VERSION.SdkInt >= BuildVersionCodes.P)
                Window.Attributes.LayoutInDisplayCutoutMode = devicesWithNotchesAllowFullScreen ? LayoutInDisplayCutoutMode.ShortEdges : LayoutInDisplayCutoutMode.Default;
        }
        #endregion

        #region CheckForPreferenceChanges
        private void CheckForPreferenceChanges()
        {
            // Check if anything has been changed in the Settings Fragment before re-reading and updating all the preference variables
            //ISharedPreferences sharedPreferences= PreferenceManager.GetDefaultSharedPreferences(this);

            sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);
            nightModeActive = sharedPreferences.GetBoolean("darkTheme", false);
            devicesWithNotchesAllowFullScreen = sharedPreferences.GetBoolean("devicesWithNotchesAllowFullScreen", false);
            animateFragments = sharedPreferences.GetBoolean("use_animations", false);
        }
        #endregion

        #region ShowSubscriptionInfoDialog
        private void ShowSubscriptionInfoDialog(string title, string explanation)
        {
            string tag = "SubscriptionInfoDialogFragment";
            AndroidX.Fragment.App.FragmentManager fm = SupportFragmentManager;
            if (fm != null && !fm.IsDestroyed)
            {
                AndroidX.Fragment.App.Fragment fragment = fm.FindFragmentByTag(tag);
                if (fragment == null)
                    BasicDialogFragment.NewInstance(title, explanation).Show(fm, tag);
            }
        }
        #endregion

    }
}