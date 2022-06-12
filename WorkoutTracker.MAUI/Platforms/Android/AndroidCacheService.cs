using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using WorkoutTracker.ViewModels;

namespace WorkoutTracker.MAUI.Android
{
    public class AndroidCacheService : ICacheService
    {
        private IEnumerable<ExerciseViewModel> _exercisesInMemoryCache;
        private string exercisesFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "exercises.json");
        private string summaryFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "workoutSummaries.json");

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

        public Task<bool> IsSummariesCached()
        {
            return Task.FromResult(File.Exists(summaryFile));
        }

        public async Task<IEnumerable<WorkoutSummary>> GetSummaries()
        {
            var json = await File.ReadAllTextAsync(summaryFile);
            return JsonSerializer.Deserialize<IEnumerable<WorkoutSummary>>(json);
        }

        public async Task SaveSummaries(IEnumerable<WorkoutSummary> summaries)
        {
            using (var writer = File.CreateText(summaryFile))
            {
                var json = JsonSerializer.Serialize(summaries);
                await writer.WriteLineAsync(json);
            }
        }

        public Task ResetSummariesCache()
        {
            File.Delete(summaryFile);

            return Task.CompletedTask;
        }
    }
}
