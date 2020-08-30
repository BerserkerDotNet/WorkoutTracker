using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Data;
using WorkoutTracker.Interfaces;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    public class ExerciseRandomizer : Android.Support.V4.App.Fragment, IActivityResultHandler
    {
        private static Random _random = new Random();
        private const string RandomizedExercises = "RandomizedExercises";
        private CacheManager _cache;
        private ListView _exercisesList;
        private IToolBarHost _toolBarHost;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.exercise_randomizer, container, false);
            _cache = new CacheManager(Context);

            _exercisesList = view.FindViewById<ListView>(Resource.Id.randomize_list);
            _exercisesList.ItemClick += OnAddExerciseLog;

            Randomize();

            var randomizeBtn = view.FindViewById<Button>(Resource.Id.randomize_exercises);
            randomizeBtn.Click += OnRandomize;

            _toolBarHost = Activity as IToolBarHost;
            if (_toolBarHost is object)
            {
                _toolBarHost.FabButtonVisible = false;
            }

            return view;
        }

        public override void OnDestroy()
        {
            _exercisesList.ItemClick -= OnAddExerciseLog;
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

            ApiRepository.Instance.Create(exerciseLogEntry).ContinueWith(t =>
            {
                Activity.RunOnUiThread(() =>
                {
                    var msg = t.IsFaulted ? "Failed to save" : "Exercise saved";
                    Toast.MakeText(Activity, msg, ToastLength.Short).Show();
                });

            });
        }

        private void OnAddExerciseLog(object sender, AdapterView.ItemClickEventArgs e)
        {
            var adapter = (RandomizeListAdapter)_exercisesList.Adapter;
            var item = adapter[e.Position];


            var intent = new Intent(Activity, typeof(AddExerciseLogActivity));
            var exerciseId = item.Id;

            intent.PutExtra(nameof(ExerciseLogEntry.ExerciseId), exerciseId.ToString());
            StartActivityForResult(intent, 1);
        }

        private void OnRandomize(object sender, EventArgs e)
        {
            InMemoryCache.Instance.Remove(RandomizedExercises);
            Randomize();
        }

        private void Randomize()
        {
            if (InMemoryCache.Instance.ContainsKey(RandomizedExercises))
            {
                var randomized = InMemoryCache.Instance.GetCollection<Exercise>(RandomizedExercises);
                DisplayExercises(randomized.ToArray());
                return;
            }

            var exercises = _cache.GetAll(ApiRepository.Instance.GetAll<Exercise>)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            var categoriesToPick = new[] { "Chest", "Back", "Shoulders", "Triceps", "Biceps", "Abdominals" };
            _random.Shuffle(categoriesToPick);

            var randomSet = new List<Exercise>(categoriesToPick.Length);

            foreach (var category in categoriesToPick)
            {
                var exercisesByCategory = exercises.Where(e => e.Muscles.Contains(category));
                var count = exercisesByCategory.Count();

                var pickedExercise = exercisesByCategory.ElementAt(_random.Next(0, count));
                randomSet.Add(pickedExercise);
            }

            InMemoryCache.Instance.SetCollection(RandomizedExercises, randomSet);

            DisplayExercises(randomSet.ToArray());
        }

        private void DisplayExercises(Exercise[] exercises)
        {
            _exercisesList.Adapter = new RandomizeListAdapter(Activity, exercises);
        }
    }
}