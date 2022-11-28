using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;

namespace WorkoutTracker.Data;

public class CachedWorkoutRepository : CosmosDbWorkoutRepository // Inheritance because internally Cosmos repo needs cached exercises
{
    private readonly ICacheService _cacheService;
    private readonly Dictionary<string, IEnumerable<LogEntryViewModel>> _logsCache = new Dictionary<string, IEnumerable<LogEntryViewModel>>();

    public CachedWorkoutRepository(IHttpClientFactory clientFactory, ICacheService cacheService, IAccessTokenProvider accessTokenProvider, ApplicationContext<IWorkoutRepository> context)
        : base(clientFactory, accessTokenProvider, context)
    {
        _cacheService = cacheService;
    }

    public override async Task<IEnumerable<ExerciseViewModel>> GetExercises()
    {
        if (await _cacheService.IsExercisesCached())
        {
            Context.LogInformation("Cache hit, returning cached exercises.");
            return await _cacheService.GetExercises();
        }

        Context.LogWarning("Cache miss, fetching exercises from server.");
        var exercises = await base.GetExercises();
        await _cacheService.SaveExercises(exercises);

        return exercises;
    }

    public override async Task<IEnumerable<WorkoutSummary>> GetWorkoutSummaries(DateTime from, DateTime to)
    {
        var isCached = await _cacheService.IsSummariesCached();
        var summaries = Enumerable.Empty<WorkoutSummary>();
        if (isCached)
        {
            summaries = await _cacheService.GetSummaries();
            from = summaries.MaxBy(s => s.Date).Date.Date.AddDays(1); // TODO: Store date with summaries
        }

        var newSummaries = await base.GetWorkoutSummaries(from, to);

        summaries = summaries.Union(newSummaries).ToArray();
        await _cacheService.SaveSummaries(summaries);

        return summaries;
    }

    public override async Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime date)
    {
        var key = date.ToString("O");
        if (_logsCache.ContainsKey(key))
        {
            return _logsCache[key];
        }

        var logs = await base.GetLogs(date);
        _logsCache[key] = logs;

        return logs;
    }

    public override Task AddLogRecord(LogEntryViewModel model)
    {
        var key = model.Date.ToString("O");
        _logsCache.Remove(key);
        return base.AddLogRecord(model);
    }
}