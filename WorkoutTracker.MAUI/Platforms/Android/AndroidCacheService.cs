using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using WorkoutTracker.ViewModels;
using System;

namespace WorkoutTracker.MAUI.Android
{
    public class AndroidCacheService : ICacheService
    {
        private IEnumerable<ExerciseViewModel> _exercisesInMemoryCache;
        private string exercisesFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "exercises.json");

        public async Task SaveExercises(IEnumerable<ExerciseViewModel> exercises)
        {
            using (var writer = File.CreateText(exercisesFile))
            {
                var json = JsonSerializer.Serialize(exercises);
                await writer.WriteLineAsync(json);
            }
        }

        public async Task<IEnumerable<ExerciseViewModel>> GetExercises()
        {
            if (_exercisesInMemoryCache is object) 
            {
                return _exercisesInMemoryCache;
            }

            var json = await File.ReadAllTextAsync(exercisesFile);
            _exercisesInMemoryCache = JsonSerializer.Deserialize<IEnumerable<ExerciseViewModel>>(json);

            return _exercisesInMemoryCache;
        }

        public Task ResetExercisesCache()
        {
            File.Delete(exercisesFile);

            return Task.CompletedTask;
        }

        public Task<bool> IsExercisesCached()
        {
            return Task.FromResult(File.Exists(exercisesFile));
        }
    }
}
