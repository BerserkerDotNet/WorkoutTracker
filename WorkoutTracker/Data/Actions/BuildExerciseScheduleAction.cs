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

        var allExercises = await _repository.GetExercises();
        
        var exerciseFiltersToShuffle = profile.ExerciseFilters.Skip(profile.ShuffleStartIndex + 1).ToArray();
        var shuffledFilters = _random.Shuffle(exerciseFiltersToShuffle);
        var exerciseFilters = profile.ExerciseFilters.Take(profile.ShuffleStartIndex + 1).Concat(shuffledFilters).ToArray();

        if (profile.IncludeCore)
        {
            exerciseFilters = exerciseFilters.Union(new MuscleGroupExerciseFilter[] { "Core" }).ToArray(); // Core is always last
        }

        var randomSet = new List<ScheduleViewModel>(exerciseFilters.Length);
        var exercisesToPickFrom = new List<ExerciseViewModel>(allExercises);

        foreach (var selector in exerciseFilters)
        {
            var selectedExercises = exercisesToPickFrom.Where(e => selector.Match(e)).ToArray();           
            var index = _random.Next(0, selectedExercises.Count());
            randomSet.Add(new ScheduleViewModel(Guid.NewGuid(), index, selectedExercises));
            exercisesToPickFrom.Remove(selectedExercises.ElementAt(index)); // At least prevent same exercise from appearing immediatly in the list
        }

        dispatcher.Dispatch(new ReceiveExerciseScheduleAction(randomSet.ToArray()));
    }
}