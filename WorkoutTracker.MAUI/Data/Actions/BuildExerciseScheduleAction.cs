using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.ViewModels;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class BuildExerciseScheduleAction : IAsyncAction<IEnumerable<ExerciseViewModel>>
    {
        private readonly Random _random = new Random();

        public Task Execute(IDispatcher dispatcher, IEnumerable<ExerciseViewModel> allExercises)
        {
            if (allExercises is null)
            {
                return Task.CompletedTask;
            }

            var categoriesToPick = new[] { "Chest", "Back", "Shoulder", "Triceps", "Biceps" };
            _random.Shuffle(categoriesToPick);
            categoriesToPick = categoriesToPick.Union(new[] { "Core" }).ToArray(); // Core is always last

            var randomSet = new List<ExerciseViewModel>(categoriesToPick.Length);

            foreach (var category in categoriesToPick)
            {
                var exercisesByCategory = allExercises.Where(e => e.Muscles.First().MuscleGroup == category);
                var count = exercisesByCategory.Count();

                var pickedExercise = exercisesByCategory.ElementAt(_random.Next(0, count));
                randomSet.Add(pickedExercise);
            }

            dispatcher.Dispatch(new ReceiveExerciseScheduleAction(randomSet.ToArray()));

            return Task.CompletedTask;
        }
    }
}
