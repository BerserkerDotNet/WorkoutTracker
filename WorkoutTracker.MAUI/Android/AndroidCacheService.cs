using WorkoutTracker.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace WorkoutTracker.MAUI.Android
{
    public class AndroidCacheService : ICacheService
    {
        private string exercisesFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "exercises.json");

        public async Task SaveExercises(IEnumerable<Exercise> exercises)
        {
            using (var writer = File.CreateText(exercisesFile))
            {
                var json = JsonSerializer.Serialize(exercises);
                await writer.WriteLineAsync(json);
            }
        }

        public async Task<IEnumerable<Exercise>> GetExercises()
        {
            var json = await File.ReadAllTextAsync(exercisesFile);
            return JsonSerializer.Deserialize<IEnumerable<Exercise>>(json);
        }

        public bool IsExercisesCached()
        {
            return File.Exists(exercisesFile);
        }
    }
}
