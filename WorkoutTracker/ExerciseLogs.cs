using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using WorkoutTracker.Data;
using WorkoutTracker.Interfaces;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    public class ExerciseLogs : Android.Support.V4.App.Fragment
    {
        ExerciseLogsAdapter _exercisesAdapter;
        IToolBarHost _toolBarHost;

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

            _toolBarHost = Activity as IToolBarHost;
            if (_toolBarHost is object)
            {
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

        private void OnFabClicked(object sender, EventArgs e)
        {
            var intent = new Intent(Activity, typeof(AddExerciseLogActivity));
            var exerciseId = _exercisesAdapter.Last.ExerciseId;

            intent.PutExtra(nameof(ExerciseLogEntry.ExerciseId), exerciseId.ToString());
            StartActivityForResult(intent, 1);
        }
    }
}