using System;
using System.Text.RegularExpressions;
using Android;
using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using WorkoutTracker.Data;
using WorkoutTracker.Interfaces;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener, IToolBarHost
    {
        private const string CurrentFragmentTag = "CURRENT_FRAGMENT";
        private FloatingActionButton _fabButton;

        public bool FabButtonVisible 
        {
            get => _fabButton.Visibility == ViewStates.Visible;
            set => _fabButton.Visibility = value ? ViewStates.Visible : ViewStates.Gone;
        }

        public event EventHandler OnFabClicked;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            _fabButton = FindViewById<FloatingActionButton>(Resource.Id.addNewExercise);
            _fabButton.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            NavigateTo<ExerciseLogs>();
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            OnFabClicked?.Invoke(sender, eventArgs);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            var handler = SupportFragmentManager.FindFragmentByTag(CurrentFragmentTag) as IActivityResultHandler;
            if (handler is object)
            {
                handler.HandleActivityResult(requestCode, resultCode, data);
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.nav_exercise_logs:
                    NavigateTo<ExerciseLogs>();
                    break;
                case Resource.Id.nav_exercises:
                    NavigateTo<Exercises>();
                    break;
            }

            var drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        private void NavigateTo<T>()
            where T: Android.Support.V4.App.Fragment, new()
        {
            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.fragment_container, new T(), CurrentFragmentTag)
                .Commit();
        }
    }
}

