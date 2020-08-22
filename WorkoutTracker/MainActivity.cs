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
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        ExerciseListAdapter _exercisesAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            var refreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.exercises_refresh_layout);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.addNewExercise);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            var toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            _exercisesAdapter = new ExerciseListAdapter(this, ApiRepository.Instance, refreshLayout);
            var exercisesLog = FindViewById<ListView>(Resource.Id.exercises_list);
            exercisesLog.Adapter = _exercisesAdapter;
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
            var intent = new Android.Content.Intent(this, typeof(AddExerciseLogActivity));
            var exerciseId = _exercisesAdapter.Last.ExerciseId;

            intent.PutExtra(nameof(ExerciseLogEntry.ExerciseId), exerciseId.ToString());
            StartActivityForResult(intent, 1);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Android.Content.Intent data)
        {
            if (resultCode == Result.Canceled)
            {
                return;
            }

            var exerciseLogEntry = new ExerciseLogEntry
            {
                Id = Guid.NewGuid(),
                ExerciseId = Guid.Parse(data.GetStringExtra("Exercise")),
                Date = DateTime.UtcNow,
                Repetitions = data.GetIntExtra("Reps", 0),
                Duration = TimeSpan.FromSeconds(data.GetIntExtra("Duration", 0)),
                Score = data.GetIntExtra("Score", 0),
                Weight = data.GetDoubleExtra("Weight", 0),
                Note = data.GetStringExtra("Note")
            };

            _exercisesAdapter.AddAndRefresh(exerciseLogEntry);
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_camera)
            {
                // Handle the camera action
            }
            else if (id == Resource.Id.nav_gallery)
            {
            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

