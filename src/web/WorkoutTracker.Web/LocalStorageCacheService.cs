using BlazorStorage.Interfaces;

namespace WorkoutTracker.Web;

public class LocalStorageCacheService : ICacheService
{
    private readonly ILocalStorage _storage;

    public LocalStorageCacheService(ILocalStorage storage)
    {
        _storage = storage;
    }

    public ValueTask<T> GetAsync<T>(string key) where T : class
    {
        return _storage.GetItem<T>(key);
    }

    public async ValueTask<bool> HasKey(string key)
    {
        var result = await _storage.GetItem<object>(key);
        return result is not null;
    }

    public ValueTask RemoveAsync(string key)
    {
        return _storage.RemoveItem(key);
    }

    public ValueTask SetAsync<T>(string key, T entry) where T : class
    {
        return _storage.SetItem<T>(key, entry);
    }
}
