using Microsoft.Extensions.Logging;

namespace WorkoutTracker.Data.Actions;

public class BuildExerciseScheduleAction : TrackableAction<ExerciseProfile>
{
    private readonly Random _random = new Random();
    private readonly IWorkoutRepository _repository;

    public BuildExerciseScheduleAction(IWorkoutRepository repository, ApplicationContext<BuildExerciseScheduleAction> context)
        : base(context)
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, ExerciseProfile profile, Dictionary<string, string> trackableProperties)
    {
        trackableProperties.Add("Profile", profile.Name);

        if (profile is null)
        {
            Context.LogWarning("Profile is empty");
            return;
        }

        Context.LogInformation("Building schedule for {Profile}", profile.Name);
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
                Context.LogWarning("Didn't find any exercises matching {Selector} in {Profile}", selector.GetType().Name, profile.Name);
                continue;
            }

            var exercises = descriptor.MatchedExercises.ToArray();
            var restTime = descriptor.TargetRestTime ?? profile.DefaultRestTime;
            var targetSets = descriptor.TargetSets ?? profile.DefaultNumberOfSets;
            var index = _random.Next(0, exercises.Length);
            randomSet.Add(new ScheduleViewModel(Guid.NewGuid(), index, targetSets, restTime, exercises));
            exercisesToPickFrom.Remove(exercises.ElementAt(index)); // At least prevent same exercise from appearing immediatly in the list
        }

        dispatcher.Dispatch(new SetCurrentSchedule(null));
        dispatcher.Dispatch(new ReceiveExerciseScheduleAction(randomSet.ToArray()));
    }
}