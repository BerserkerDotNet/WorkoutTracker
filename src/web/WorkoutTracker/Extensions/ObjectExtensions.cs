using System.Text.Json;

namespace WorkoutTracker.Extensions;

public static class ObjectExtensions
{
    public static T Clone<T>(this T obj)
        where T : class
    {
        return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(obj));
    }
}
