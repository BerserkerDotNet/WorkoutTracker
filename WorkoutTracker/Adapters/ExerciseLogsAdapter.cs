using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    public class ExerciseLogsAdapter : BaseAdapter<ExerciseLogEntry>
    {
        private readonly Activity _context;
        private readonly IRepository _repository;
        private readonly SwipeRefreshLayout _refreshLayout;
        private ExerciseLogEntry[] _items;
        private Dictionary<Guid, Exercise> _exercisesLookup;

        public ExerciseLogsAdapter(Activity context, IRepository repository, SwipeRefreshLayout refreshLayout)
        {
            _context = context;
            _repository = repository;
            _refreshLayout = refreshLayout;
            _refreshLayout.Refresh += OnRefresh;

            _items = Array.Empty<ExerciseLogEntry>();

            ReloadData(loadExercises: true);
        }

        public override ExerciseLogEntry this[int position] => _items[position];

        public override int Count => _items.Length;

        public ExerciseLogEntry Last => _items[0];

        public override long GetItemId(int position)
        {
            return _items[position].GetHashCode();
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = _context.LayoutInflater.Inflate(Resource.Layout.exerciselogs_listview, null);
            }

            var log = this[position];
            var exercise = _exercisesLookup[log.ExerciseId];
            var nameField = convertView.FindViewById<TextView>(Resource.Id.exercise_name);
            var dateField = convertView.FindViewById<TextView>(Resource.Id.exercise_date);
            var statsField = convertView.FindViewById<TextView>(Resource.Id.exercise_stats);
            var scoreField = convertView.FindViewById<TextView>(Resource.Id.exercise_score);
            var iconField = convertView.FindViewById<ImageView>(Resource.Id.exercise_icon);
            var icon = _context.Resources.GetIdentifier(exercise.Icon, "drawable", _context.PackageName);
            nameField.Text = exercise.Name;
            dateField.Text = log.Date.ToLocalTime().ToString("f");
            statsField.Text = $"Reps: {log.Repetitions} Weight: {log.Weight} Duration: {log.Duration}";
            scoreField.Text = $"Score: {log.Score}";

            iconField.SetImageResource(icon);
            return convertView;
        }

        public Task ReloadData(bool loadExercises = false)
        {
            _refreshLayout.Refreshing = true;

            return _repository.GetAll<ExerciseLogEntry>().ContinueWith(itemsTask =>
            {
                if (loadExercises)
                {
                    var exercises = _repository.GetAll<Exercise>().ConfigureAwait(false).GetAwaiter().GetResult();
                    _exercisesLookup = exercises.ToDictionary(e => e.Id, e => e);
                    InMemoryCache.Instance.SetCollection(nameof(Exercise), exercises);
                }

                _items = itemsTask.Result.OrderByDescending(i => i.Date).ToArray();
                _context.RunOnUiThread(() =>
                {
                    NotifyDataSetChanged();
                    _refreshLayout.Refreshing = false;
                });
            });
        }

        public Task AddAndRefresh(ExerciseLogEntry record)
        {
            _refreshLayout.Refreshing = true;
            return _repository.Create(record).ContinueWith(_ => ReloadData());
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            ReloadData();
        }
    }
}

