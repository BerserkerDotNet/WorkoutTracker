using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    public class ExerciseLogsAdapter : BaseAdapter<ExerciseLogEntry>, AbsListView.IOnScrollListener
    {
        private readonly Activity _context;
        private readonly IExerciseLogRepository _repository;
        private readonly SwipeRefreshLayout _refreshLayout;
        private List<ExerciseLogEntry> _items;
        private DateTime[] _dates;
        private int _currentDateIndex;
        private Dictionary<Guid, Exercise> _exercisesLookup;

        public ExerciseLogsAdapter(Activity context, IExerciseLogRepository repository, SwipeRefreshLayout refreshLayout)
        {
            _context = context;
            _repository = repository;
            _refreshLayout = refreshLayout;
            _refreshLayout.Refresh += OnRefresh;

            _items = new List<ExerciseLogEntry>();
            _dates = Array.Empty<DateTime>();

            ReloadData();
        }

        public override ExerciseLogEntry this[int position] => _items[position];

        public override int Count => _items.Count;

        public ExerciseLogEntry Last => _items.Count > 0 ? _items[0] : null;

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
            var iconBitmap = BitmapFactory.DecodeByteArray(exercise.Icon, 0, exercise.Icon.Length);
            nameField.Text = exercise.Name;
            dateField.Text = log.Date.ToLocalTime().ToString("f");
            statsField.Text = $"Reps: {log.Repetitions} Weight: {log.Weight} Duration: {log.Duration}";
            scoreField.Text = $"Score: {log.Score}";

            iconField.SetImageBitmap(iconBitmap);

            return convertView;
        }

        private Task ReloadData()
        {
            _refreshLayout.Refreshing = true;
            var exercisesTask = InMemoryCache.Instance.ContainsKey(nameof(Exercise)) ? Task.FromResult(InMemoryCache.Instance.GetCollection<Exercise>(nameof(Exercise))) : _repository.GetAll<Exercise>();
            var datesTask = _repository.GetDates();

            return Task.WhenAll(exercisesTask, datesTask)
                .ContinueWith(t =>
                {
                    var exercises = exercisesTask.Result;
                    _exercisesLookup = exercises.ToDictionary(e => e.Id, e => e);
                    InMemoryCache.Instance.SetCollection(nameof(Exercise), exercises);

                    _dates = datesTask.Result
                        .Select(d => DateTime.ParseExact(d, "dd-MM-yyyy", null))
                        .OrderByDescending(d => d)
                        .ToArray();
                    _currentDateIndex = 0;
                    _items.Clear();

                }).ContinueWith(_ => LoadNextChunk());
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

        private Task LoadNextChunk()
        {
            if (_dates.Length <= _currentDateIndex)
            {
                return Task.CompletedTask;
            }

            if (_dates.Length == 0)
            {
                StopLoadingAndNotifyChange();
                return Task.CompletedTask;
            }

            _refreshLayout.Refreshing = true;

            return _repository.GetByDate(_dates[_currentDateIndex]).ContinueWith(chunk =>
            {
                _items.AddRange(chunk.Result.OrderByDescending(e => e.Date));
                _currentDateIndex++;
                StopLoadingAndNotifyChange();
            });
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
            if (_refreshLayout.Refreshing)
            {
                return;
            }

            if (firstVisibleItem + visibleItemCount >= totalItemCount)
            {
                LoadNextChunk();
            }
        }

        public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] ScrollState scrollState)
        {
        }

        private void StopLoadingAndNotifyChange()
        {
            _context.RunOnUiThread(() =>
            {
                NotifyDataSetChanged();
                _refreshLayout.Refreshing = false;
            });
        }
    }
}

