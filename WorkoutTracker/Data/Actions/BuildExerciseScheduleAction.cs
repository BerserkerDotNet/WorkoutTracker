using WorkoutTracker.Extensions;

namespace WorkoutTracker.Data.Actions;

public class BuildExerciseScheduleAction : IAsyncAction<ExerciseProfile>
{
    private readonly Random _random = new Random();
    private readonly IWorkoutRepository _repository;

    public BuildExerciseScheduleAction(IWorkoutRepository repository)
    {
        _repository = repository;
    }

    public async Task Execute(IDispatcher dispatcher, ExerciseProfile profile)
    {
        if (profile is null)
        {
            return;
        }

        var exercises = await _repository.GetExercises();

        var categoriesToPick = _random.Shuffle(profile.MuscleGroups);
        if (profile.IncludeCore)
        {
            categoriesToPick = categoriesToPick.Union(new[] { "Core" }).ToArray(); // Core is always last
        }

        var randomSet = new Dictionary<string, ScheduleViewModel>(categoriesToPick.Length);

        foreach (var category in categoriesToPick)
        {
            var exercisesByCategory = exercises.Where(e => e.Muscles.First().MuscleGroup == category).ToArray();
            var index = _random.Next(0, exercisesByCategory.Count());
            randomSet.Add(category, new ScheduleViewModel(category, index, exercisesByCategory));
        }

        dispatcher.Dispatch(new ReceiveExerciseScheduleAction(randomSet));
    }
}