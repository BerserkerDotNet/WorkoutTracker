using WorkoutTracker.Extensions;

namespace WorkoutTracker.Data.Actions
{
    public class BuildUpperBodyExerciseScheduleAction : IAsyncAction<IEnumerable<ExerciseViewModel>>
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

            var randomSet = new Dictionary<string, ScheduleViewModel>(categoriesToPick.Length);

            foreach (var category in categoriesToPick)
            {
                var exercisesByCategory = allExercises.Where(e => e.Muscles.First().MuscleGroup == category).ToArray();
                var index = _random.Next(0, exercisesByCategory.Count());
                randomSet.Add(category, new ScheduleViewModel(category, index, exercisesByCategory));
            }

            dispatcher.Dispatch(new ReceiveExerciseScheduleAction(randomSet));

            return Task.CompletedTask;
        }
    }
}
