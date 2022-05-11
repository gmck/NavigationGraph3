using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;
using AndroidX.Navigation;

namespace com.companyname.NavigationGraph3.Fragments
{
    // The comments here also apply equally to the LeaderboardFragment 
    // By default because the LeaderboardFragment and the RegisterFragment are not top level fragments, they will default to showing an up button or left arrow on the toolbar plus the title of the fragment
    // If you don't want the up button, it can be removed in MainActivity's OnDestinationChange - see example there.
    // This also means that the additional code in OnSupportNavigationUp can be removed. The code in OnSupportNavigationUp is there for when you do want for whatever reason want to retain the Up buttom.
    // All it does is to ensure that a fragment containing an Up button can still be directed away from the standard action of an up button, which is to always return when closing to the StartDestination Fragment.

    public class RegisterFragment : Fragment
    {
        private NavFragmentOnBackPressedCallback onBackPressedCallback;
       
        public RegisterFragment() { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            View view = inflater.Inflate(Resource.Layout.fragment_register, container, false);
            TextView textView = view.FindViewById<TextView>(Resource.Id.text_register);
            textView.Text = "This is Register fragment";
            return view;
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            // Don't want a menu
            base.OnCreateOptionsMenu(menu, inflater);
            menu.Clear();
        }

        #region OnResume
        public override void OnResume()
        {
            base.OnResume();

            onBackPressedCallback = new NavFragmentOnBackPressedCallback(this, true);
            //// Android docs:  Strongly recommended to use the ViewLifecycleOwner.This ensures that the OnBackPressedCallback is only added when the LifecycleOwner is Lifecycle.State.STARTED.
            //// The activity also removes registered callbacks when their associated LifecycleOwner is destroyed, which prevents memory leaks and makes it suitable for use in fragments or other lifecycle owners
            //// that have a shorter lifetime than the activity.
            //// Note: this rule out using OnAttach(Context context) as the view hasn't been created yet.
            RequireActivity().OnBackPressedDispatcher.AddCallback(ViewLifecycleOwner, onBackPressedCallback);
        }
        #endregion

        #region OnDestroy
        public override void OnDestroy()
        {
            onBackPressedCallback?.Remove();
            base.OnDestroy();
        }
        #endregion

        #region HandleBackPressed
        public void HandleBackPressed(NavOptions navOptions)
        {
            onBackPressedCallback.Enabled = false;

            NavController navController = Navigation.FindNavController(Activity, Resource.Id.nav_host);
            
            // Always Navigate back to the SlideShowFragment
            navController.PopBackStack(Resource.Id.slideshow_fragment, false);
            navController.Navigate(Resource.Id.slideshow_fragment, null, navOptions);
        }
        #endregion
    }
}