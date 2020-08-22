using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    [Activity(Label = "AddExerciseLogActivity")]
    public class AddExerciseLogActivity : Activity
    {
        private Stopwatch _durationStopwatch;
        private EditText _durationField;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var exerciseid = Guid.Parse(Intent.GetStringExtra(nameof(ExerciseLogEntry.ExerciseId)));

            _durationStopwatch = new Stopwatch();

            SetContentView(Resource.Layout.exerciselogs_add);

            _durationField = FindViewById<EditText>(Resource.Id.add_exercise_duration);

            // TODO: unsubscribe?
            var exerciseAdapter = new ExerciseSelector(this);
            var exercise = FindViewById<Spinner>(Resource.Id.add_exercise_exerciseid);
            exercise.Adapter = exerciseAdapter;

            exercise.SetSelection(exerciseAdapter.GetPositionById(exerciseid));

            var startStopButton = FindViewById<ToggleButton>(Resource.Id.durationStartStop);
            startStopButton.CheckedChange += OnStartStopMeasureTime;

            var saveButton = FindViewById<Button>(Resource.Id.add_exercise_save);
            saveButton.Click += OnSave;

            var cancelButton = FindViewById<Button>(Resource.Id.add_exercise_cancel);
            cancelButton.Click += OnCancel;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnStartStopMeasureTime(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (e.IsChecked)
            {
                _durationStopwatch.Reset();
                _durationStopwatch.Start();
            }
            else
            {
                _durationStopwatch.Stop();

                _durationField.Text = $"{(int)Math.Round(_durationStopwatch.Elapsed.TotalSeconds, 0)}";
            }
        }

        private void OnSave(object sender, EventArgs e)
        {
            var exercise = FindViewById<Spinner>(Resource.Id.add_exercise_exerciseid);
            var repsField = FindViewById<EditText>(Resource.Id.add_exercise_reps);
            var weightField = FindViewById<EditText>(Resource.Id.add_exercise_weight);
            var weightUnitsSwitch = FindViewById<ToggleButton>(Resource.Id.weightUnits);

            var scoreField = FindViewById<RatingBar>(Resource.Id.add_exercise_score);
            var noteField = FindViewById<EditText>(Resource.Id.add_exercise_note);
            var data = new Intent();

            var reps = string.IsNullOrEmpty(repsField.Text) ? 0 : int.Parse(repsField.Text);
            var weight = string.IsNullOrEmpty(weightField.Text) ? 0 : double.Parse(weightField.Text);
            var duration = string.IsNullOrEmpty(_durationField.Text) ? 0 : int.Parse(_durationField.Text);

            var selectedExercise = ObjectTypeHelper.Cast<Exercise>(exercise.SelectedItem);

            if (weightUnitsSwitch.Checked)
            {
                weight = weight * 0.453592;
            }

            data.PutExtra("Exercise", selectedExercise.Id.ToString());
            data.PutExtra("Reps", reps);
            data.PutExtra("Weight", weight);
            data.PutExtra("Duration", duration);
            data.PutExtra("Score", (int)scoreField.Rating);
            data.PutExtra("Note", noteField.Text);
            SetResult(Result.Ok, data);
            Finish();
        }

        private void OnCancel(object sender, EventArgs e)
        {
            SetResult(Result.Canceled);
            Finish();
        }
    }

    public static class ObjectTypeHelper
    {
        public static T Cast<T>(this Java.Lang.Object obj) where T : class
        {
            var propertyInfo = obj.GetType().GetProperty("Instance");
            return propertyInfo == null ? null : propertyInfo.GetValue(obj, null) as T;
        }
    }
}