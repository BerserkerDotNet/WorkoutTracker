using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using WorkoutTracker.Data;
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

    public class ExerciseSelector : BaseAdapter<Exercise>
    {
        private readonly Activity _context;
        Exercise[] _items;

        public ExerciseSelector(Activity context)
        {
            var exercises = InMemoryCache.Instance.GetCollection<Exercise>(nameof(Exercise));
            _items = exercises.ToArray();

            _context = context;
        }

        public override Exercise this[int position] => _items[position];

        public override int Count => _items.Length;

        public override long GetItemId(int position)
        {
            return _items[position].Id.GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = _items[position];
            var layout = _context.LayoutInflater.Inflate(Resource.Layout.exercise_selector, null);

            var img = layout.FindViewById<ImageView>(Resource.Id.exercise_selector_image);
            var name = layout.FindViewById<TextView>(Resource.Id.exercise_selector_name);

            var icon = _context.Resources.GetIdentifier(item.Icon, "drawable", _context.PackageName);
            img.SetImageResource(icon);

            name.Text = item.Name;

            return layout;
        }

        public int GetPositionById(Guid id)
        {
            var item = _items.Single(i => i.Id == id);
            return Array.IndexOf(_items, item);
        }
    }
}