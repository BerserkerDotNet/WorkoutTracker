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

        var allExercises = await _repository.GetExercises();
        var exerciseFilters = profile.Shuffler.Shuffle(profile.ExerciseSelectors);

        if (profile.IncludeCore)
        {
            exerciseFilters = exerciseFilters.Union(new MuscleGroupExerciseSelector[] { "Core" }).ToArray(); // Core is always last
        }

        var randomSet = new List<ScheduleViewModel>(exerciseFilters.Count());
        var exercisesToPickFrom = new List<ExerciseViewModel>(allExercises);

        foreach (var selector in exerciseFilters)
        {
            var descriptor = selector.Select(exercisesToPickFrom);
            if (descriptor is null) 
            {
                continue;
            }

            var exercises = descriptor.MatchedExercises.ToArray();
            var restTime = descriptor.TargetRestTime ?? profile.DefaultRestTime;
            var targetSets = descriptor.TargetSets ?? profile.DefaultNumberOfSets;
            var index = _random.Next(0, exercises.Length);
            randomSet.Add(new ScheduleViewModel(Guid.NewGuid(), index, targetSets, restTime, exercises));
            exercisesToPickFrom.Remove(exercises.ElementAt(index)); // At least prevent same exercise from appearing immediatly in the list
        }

        dispatcher.Dispatch(new ReceiveExerciseScheduleAction(randomSet.ToArray()));
    }
}