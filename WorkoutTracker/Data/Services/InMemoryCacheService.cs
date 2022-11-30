namespace WorkoutTracker.Data.Services;

public class NullCacheService : ICacheService
{
    public ValueTask<T> GetAsync<T>(string key) where T : class
    {
        return ValueTask.FromResult<T>(default);
    }

    public ValueTask<bool> HasKey(string key)
    {
        return ValueTask.FromResult(false);
    }

    public ValueTask RemoveAsync(string key)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask SetAsync<T>(string key, T entry) where T : class
    {
        return ValueTask.CompletedTask;
    }
}
