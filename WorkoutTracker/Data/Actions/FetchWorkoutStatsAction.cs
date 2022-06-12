using UnitsNet;

namespace WorkoutTracker.Data.Actions;

public record WorkoutStatsRequest(DateTime From, DateTime To);

public class FetchWorkoutStatsAction : TrackableAction<WorkoutStatsRequest>
{
    private readonly IWorkoutRepository _repository;
    private readonly ICacheService _cacheService;

    public FetchWorkoutStatsAction(IWorkoutRepository repository, ICacheService cacheService, ApplicationContext<FetchWorkoutStatsAction> context)
        : base(context)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    protected override async Task Execute(IDispatcher dispatcher, WorkoutStatsRequest request, Dictionary<string, string> trackableProperties)
    {
        var isCached = await _cacheService.IsSummariesCached();
        var summaries = Enumerable.Empty<WorkoutSummary>();
        if (isCached)
        {
            summaries = await _cacheService.GetSummaries();
            var newFromDate = summaries.MaxBy(s => s.Date).Date.Date.AddDays(1); // TODO: Store date with summaries
            request = request with { From = newFromDate };
        }

        var newSummaries = await GetSummaries(request);

        summaries = summaries.Union(newSummaries).ToArray();
        await _cacheService.SaveSummaries(summaries);

        dispatcher.Dispatch(new ReceiveWorkoutStatsAction(summaries));
    }

    private async Task<IEnumerable<WorkoutSummary>> GetSummaries(WorkoutStatsRequest request)
    {
        var logs = await _repository.GetLogs(request.From.ToUniversalTime(), request.To.ToUniversalTime());
        var summaries = logs.Where(log => log.Sets.Any()).Select(log =>
        {
            var sets = log.Sets;
            var maxSet = sets.MaxBy(s => s.WeightLB);
            var minSet = sets.MinBy(s => s.WeightLB);
            var avgWeightLb = sets.Aggregate(0.0d, (acc, s) => acc + s.WeightLB ?? 0) / sets.Count();
            var avgReps = sets.Aggregate(0, (acc, s) => acc + s.Repetitions) / sets.Count();
            var avgDuration = sets.Aggregate(TimeSpan.Zero, (acc, s) => acc + s.Duration) / sets.Count();
            var avgRest = sets.Aggregate(TimeSpan.Zero, (acc, s) => acc + s.RestTime) / sets.Count();

            var max = new WorkoutSetSummary(maxSet.WeightKG ?? 0, Math.Ceiling(maxSet.WeightLB ?? 0), maxSet.Repetitions, maxSet.Duration, maxSet.RestTime);
            var min = new WorkoutSetSummary(minSet.WeightKG ?? 0, Math.Ceiling(minSet.WeightLB ?? 0), minSet.Repetitions, minSet.Duration, minSet.RestTime);
            var avg = new WorkoutSetSummary(Mass.FromPounds(avgWeightLb).Kilograms, Math.Ceiling(avgWeightLb), avgReps, avgDuration, avgRest);
            var total = new WorkoutSetSummary(
                sets.Sum(s => s.WeightKG ?? 0),
                sets.Sum(s => s.WeightLB ?? 0),
                sets.Sum(s => s.Repetitions),
                TimeSpan.FromSeconds(sets.Sum(s => s.Duration.TotalSeconds)),
                TimeSpan.FromSeconds(sets.Sum(s => s.RestTime.TotalSeconds)));

            return new WorkoutSummary(log.Date, max, min, avg, total, sets.Count(), log.Exercise.Id);
        }).ToArray();

        return summaries;
    }
}