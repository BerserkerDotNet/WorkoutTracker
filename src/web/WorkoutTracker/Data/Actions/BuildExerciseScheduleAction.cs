using Microsoft.Extensions.Logging;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Data.Actions;

public class BuildExerciseScheduleAction : TrackableAction<ExerciseProfile>
{
    private readonly IWorkoutRepository _repository;
    private readonly WorkoutSetsService _workoutSetsService;

    public BuildExerciseScheduleAction(IWorkoutRepository repository, WorkoutSetsService workoutSetsService, ApplicationContext<BuildExerciseScheduleAction> context)
        : base(context, "Building schedule")
    {
        _repository = repository;
        _workoutSetsService = workoutSetsService;
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
        var exerciseFilters = profile.Shuffle.Shuffle(profile.ExerciseSelectors);

        if (profile.IncludeCore)
        {
            exerciseFilters = exerciseFilters.Union(new MuscleGroupExerciseSelector[] { "Core" }).ToArray(); // Core is always last
        }

        var randomSet = new List<WorkoutViewModel>(exerciseFilters.Count());
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
            var restTime = descriptor.TargetRestTime ?? profile.RestTime;
            var targetSets = descriptor.TargetSets ?? profile.NumberOfSets;
            var index = Random.Shared.Next(0, exercises.Length);
            var exerciseToPick = exercises.ElementAt(index);
            var sets = await _workoutSetsService.GenerateSets(exerciseToPick.Id, targetSets);
            var workoutExercise = new WorkoutExerciseViewModel(exerciseToPick.Id, exerciseToPick.Name, exerciseToPick.ImagePath, sets);
            randomSet.Add(new WorkoutViewModel(Guid.NewGuid(), restTime, workoutExercise));
            exercisesToPickFrom.Remove(exercises.ElementAt(index)); // At least prevent same exercise from appearing immediatly in the list
        }

        dispatcher.Dispatch(new ReceiveExerciseScheduleAction(randomSet.ToArray()));
    }
}

public class WorkoutSetsService
{
    private readonly IWorkoutRepository _repository;
    private readonly ApplicationContext<WorkoutSetsService> _context;
    private Dictionary<Guid, WorkoutSummary> _lastMaxLookup;

    public WorkoutSetsService(IWorkoutRepository repository, ApplicationContext<WorkoutSetsService> context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<IEnumerable<WorkoutExerciseSetViewModel>> GenerateSets(Guid exerciseId, int setsCount)
    {
        if (_lastMaxLookup is null)
        {
            _context.LogInformation("Fetching lookups");
            var summaries = await _repository.GetWorkoutSummaries(DateTime.UtcNow.AddMonths(-6), DateTime.UtcNow.AddDays(-1));
            _lastMaxLookup = summaries.GroupBy(s => s.ExerciseId).ToDictionary(k => k.Key, v => v.OrderByDescending(s => s.Date).First());
        }

        var todayLogs = await _repository.GetLogs(DateTime.Today);
        var todaySetsLookup = todayLogs.ToDictionary(k => k.Exercise.Id, v => v.Sets);

        var completedSets = todaySetsLookup.ContainsKey(exerciseId) ? todaySetsLookup[exerciseId] : Enumerable.Empty<Set>();
        var lastMax = _lastMaxLookup.ContainsKey(exerciseId) ? _lastMaxLookup[exerciseId].Max : new WorkoutSetSummary(0, 0, 0, TimeSpan.Zero, TimeSpan.Zero, Enumerable.Empty<Set>());
        var result = new List<WorkoutExerciseSetViewModel>(setsCount);
        for (int i = 0; i < setsCount; i++)
        {
            if (i >= completedSets.Count())
            {
                var set = new WorkoutExerciseSetViewModel(i, SetStatus.NotStarted, lastMax.WeightLb, lastMax.Repetitions, TimeSpan.Zero, TimeSpan.Zero);
                result.Add(set);
            }
            else
            {

                var completedSet = completedSets.ElementAt(i);
                var set = new WorkoutExerciseSetViewModel(i, SetStatus.Completed, completedSet.WeightLB ?? 0, completedSet.Repetitions, completedSet.RestTime, completedSet.Duration);
                result.Add(set);
            }
        }

        return result;
    }
}