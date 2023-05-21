using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.MAUI.Android;

public class AndroidCacheService : ICacheService
{
    public async ValueTask<T> GetAsync<T>(string key) where T : class
    {
        var json = await File.ReadAllTextAsync(GetFilePath(key));
        return JsonSerializer.Deserialize<T>(json);
    }

    public async ValueTask SetAsync<T>(string key, T entry) where T : class
    {
        using (var writer = File.CreateText(GetFilePath(key)))
        {
            var json = JsonSerializer.Serialize(entry);
            await writer.WriteAsync(json);
        }
    }

    public ValueTask RemoveAsync(string key)
    {
        File.Delete(GetFilePath(key));
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> HasKey(string key)
    {
        return ValueTask.FromResult(File.Exists(GetFilePath(key)));
    }

    private string GetFilePath(string key) => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"{key}.json");
}
