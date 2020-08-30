using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker
{
    public class ExerciseRandomizer : Android.Support.V4.App.Fragment
    {
        private static Random _random = new Random();
        private const string RandomizedExercises = "RandomizedExercises";

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.exercise_randomizer, container, false);

            Randomize(view);

            var randomizeBtn = view.FindViewById<Button>(Resource.Id.randomize_exercises);
            randomizeBtn.Click += OnRandomize;

            return view;
        }

        private void OnRandomize(object sender, EventArgs e)
        {
            InMemoryCache.Instance.Remove(RandomizedExercises);
            Randomize(View);
        }

        private void Randomize(View view)
        {
            if (InMemoryCache.Instance.ContainsKey(RandomizedExercises))
            {
                var randomized = InMemoryCache.Instance.GetCollection<Exercise>(RandomizedExercises);
                DisplayExercises(view, randomized.ToArray());
                return;
            }

            var exercises = InMemoryCache.Instance.GetCollection<Exercise>(nameof(Exercise));

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

            DisplayExercises(view, randomSet.ToArray());
        }

        private void DisplayExercises(View view, Exercise[] exercises)
        {
            var exercisesList = view.FindViewById<ListView>(Resource.Id.randomize_list);
            exercisesList.Adapter = new RandomizeListAdapter(Activity, exercises);
        }
    }
}