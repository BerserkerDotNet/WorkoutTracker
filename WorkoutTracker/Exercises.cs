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

namespace WorkoutTracker
{
    public class Exercises : Android.Support.V4.App.Fragment
    {
        private ExercisesAdapter _exercisesAdapter;
        private IToolBarHost _toolBarHost;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.exercises, container, false);

            var refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.exercises_refresh);
            _exercisesAdapter = new ExercisesAdapter(Activity, refreshLayout);
            var exercises = view.FindViewById<ListView>(Resource.Id.exercises_listview);
            exercises.Adapter = _exercisesAdapter;

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
        }
    }
}