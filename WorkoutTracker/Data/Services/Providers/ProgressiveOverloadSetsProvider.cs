using UnitsNet;
using WorkoutTracker.Models;
using WorkoutTracker.ViewModels.Workout;

namespace WorkoutTracker.Data.Services.SetProviders;

public class ProgressiveOverloadSetsProvider : ISetsProvider
{
    private readonly IWorkoutRepository _repository;

    public ProgressiveOverloadSetsProvider(IWorkoutRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<WorkoutSet>> Generate(Guid exerciseId)
    {
        throw new NotImplementedException();
    }

    public Task<SetsOverloadDecision> ShouldOverload(Guid exerciseId, IEnumerable<WorkoutSet> lastSets)
    {
        throw new NotImplementedException();
    }
}

public class WorkoutSummaryProvider
{
    private readonly IWorkoutRepository _repository;
    private readonly ICacheService _cacheService;

    public WorkoutSummaryProvider(IWorkoutRepository repository, ICacheService cacheService)
    {
        this._repository = repository;
        this._cacheService = cacheService;
    }

    public async Task<IEnumerable<WorkoutSummary>> GetSummaries(DateTime from, DateTime to)
    {
        var logs = await _repository.GetLogs(from.ToUniversalTime(), to.ToUniversalTime());
        var summaries = logs.Where(log => log.Sets.Any()).Select(log =>
        {
            var sets = log.Sets;
            var maxSet = sets.MaxBy(s => s.WeightLB);
            var minSet = sets.MinBy(s => s.WeightLB);
            var avgWeightLb = sets.Aggregate(0.0d, (acc, s) => acc + s.WeightLB ?? 0) / sets.Count();
            var avgReps = sets.Aggregate(0, (acc, s) => acc + s.Repetitions) / sets.Count();
            var avgDuration = sets.Aggregate(TimeSpan.Zero, (acc, s) => acc + s.Duration) / sets.Count();
            var avgRest = sets.Aggregate(TimeSpan.Zero, (acc, s) => acc + s.RestTime) / sets.Count();

            var max = new WorkoutSetSummary(maxSet.WeightKG ?? 0, Math.Ceiling(maxSet.WeightLB ?? 0), maxSet.Repetitions, maxSet.Duration, maxSet.RestTime, sets);
            var min = new WorkoutSetSummary(minSet.WeightKG ?? 0, Math.Ceiling(minSet.WeightLB ?? 0), minSet.Repetitions, minSet.Duration, minSet.RestTime, sets);
            var avg = new WorkoutSetSummary(Mass.FromPounds(avgWeightLb).Kilograms, Math.Ceiling(avgWeightLb), avgReps, avgDuration, avgRest, sets);
            var total = new WorkoutSetSummary(
                sets.Sum(s => s.WeightKG ?? 0),
                sets.Sum(s => s.WeightLB ?? 0),
                sets.Sum(s => s.Repetitions),
                TimeSpan.FromSeconds(sets.Sum(s => s.Duration.TotalSeconds)),
                TimeSpan.FromSeconds(sets.Sum(s => s.RestTime.TotalSeconds)), Enumerable.Empty<Set>());

            return new WorkoutSummary(log.Date, max, min, avg, total, sets.Count(), log.Exercise.Id);
        }).ToArray();

        return summaries;
    }
}
