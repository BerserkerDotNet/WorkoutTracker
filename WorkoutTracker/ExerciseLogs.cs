using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using WorkoutTracker.Data;
using WorkoutTracker.Interfaces;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    public class ExerciseLogs : Android.Support.V4.App.Fragment, IActivityResultHandler
    {
        private ExerciseLogsAdapter _exercisesAdapter;
        private IToolBarHost _toolBarHost;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view =  inflater.Inflate(Resource.Layout.exercises_log, container, false);

            var refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.exercises_log_refresh_layout);
            _exercisesAdapter = new ExerciseLogsAdapter(Activity, ApiRepository.Instance, refreshLayout);
            var exercisesLog = view.FindViewById<ListView>(Resource.Id.exercises_log_list);
            exercisesLog.Adapter = _exercisesAdapter;
            exercisesLog.SetOnScrollListener(_exercisesAdapter);

            _toolBarHost = Activity as IToolBarHost;
            if (_toolBarHost is object)
            {
                _toolBarHost.FabButtonVisible = true;
                _toolBarHost.OnFabClicked += OnFabClicked;
            }

            return view;
        }

        public override void OnDestroy()
        {
            if (_toolBarHost is object)
            {
                _toolBarHost.OnFabClicked -= OnFabClicked;
            }

            base.OnDestroy();
        }

        public void HandleActivityResult(int requestCode, Result resultCode, Intent data)
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

        private void OnFabClicked(object sender, EventArgs e)
        {
            var intent = new Intent(Activity, typeof(AddExerciseLogActivity));
            var exerciseId = _exercisesAdapter.Last.ExerciseId;

            intent.PutExtra(nameof(ExerciseLogEntry.ExerciseId), exerciseId.ToString());
            StartActivityForResult(intent, 1);
        }
    }
}