using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class BuildExerciseScheduleAction : IAsyncAction<IEnumerable<Exercise>>
    {
        private readonly Random _random = new Random();

        public Task Execute(IDispatcher dispatcher, IEnumerable<Exercise> allExercises)
        {
            if (allExercises is null)
            {
                return Task.CompletedTask;
            }

            var exercises = allExercises.Where(e => e.Tags.Contains("GoodForHome"));

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

            dispatcher.Dispatch(new ReceiveExerciseScheduleAction(randomSet.ToArray()));

            return Task.CompletedTask;
        }
    }
}
