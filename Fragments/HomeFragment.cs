using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.Fragment.App;

namespace com.companyname.NavigationGraph3.Fragments
{
    public class HomeFragment : Fragment
    {
        private NavFragmentOnBackPressedCallback onBackPressedCallback;

        public HomeFragment() { }
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_home, container, false);
            TextView textView = view.FindViewById<TextView>(Resource.Id.text_home);
            textView.Text = "This is home fragment";
            return view;
        }

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

        #region OnDestroy
        public override void OnDestroy()
        {
            onBackPressedCallback?.Remove();
            base.OnDestroy();
        }
        #endregion

        #region HandleBackPressed
        public void HandleBackPressed()
        {
            onBackPressedCallback.Enabled = false;

            // Had to add this for Android 12 devices becausue MainActivity's OnDestroy wasn't being called.
            // and therefore our Service plus its Notification wasn't being closed.
            Activity.Finish();
        }
        #endregion
    }


}