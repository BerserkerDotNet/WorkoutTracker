using BlazorStorage.Interfaces;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.Web;

public class LocalStorageCacheService : ICacheService
{
    private const string ExercisesKey = "WT_EXERCISES";
    private const string SummariesKey = "WT_SUMMARIES";
    private readonly ILocalStorage _storage;

    public LocalStorageCacheService(ILocalStorage storage)
    {
        _storage = storage;
    }

    public async Task<IEnumerable<ExerciseViewModel>> GetExercises()
    {
        return await _storage.GetItem<IEnumerable<ExerciseViewModel>>(ExercisesKey);
    }

    public async Task ResetExercisesCache()
    {
        await _storage.RemoveItem(ExercisesKey);
    }

    public async Task SaveExercises(IEnumerable<ExerciseViewModel> exercises)
    {
        await _storage.SetItem(ExercisesKey, exercises);
    }

    public async Task<bool> IsExercisesCached()
    {
        var exercises = await GetExercises();
        return exercises is object;
    }

    public async Task ResetSummariesCache()
    {
        await _storage.RemoveItem(SummariesKey);
    }

    public async Task SaveSummaries(IEnumerable<WorkoutSummary> summaries)
    {
        await _storage.SetItem(SummariesKey, summaries);
    }

    public async Task<IEnumerable<WorkoutSummary>> GetSummaries()
    {
        return await _storage.GetItem<IEnumerable<WorkoutSummary>>(SummariesKey);
    }

    public async Task<bool> IsSummariesCached()
    {
        var exercises = await GetSummaries();
        return exercises is object;
    }
}
