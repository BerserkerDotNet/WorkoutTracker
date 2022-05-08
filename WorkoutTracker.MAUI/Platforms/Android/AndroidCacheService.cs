using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using WorkoutTracker.ViewModels;
using System;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WorkoutTracker.MAUI.Android
{
    public class AndroidCacheService : ICacheService
    {
        private IEnumerable<ExerciseViewModel> _exercisesInMemoryCache;
        private AccessToken _token;
        private string exercisesFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "exercises.json");
        private string tokenFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "token.json");

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

        public async Task<AccessToken> GetToken()
        {
            if (_token is object) 
            {
                return _token;
            }

            if (!File.Exists(tokenFile)) 
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(tokenFile);
            _token = JsonSerializer.Deserialize<AccessToken>(json);

            return _token;
        }

        public async Task SaveToken(AccessToken token)
        {
            using (var writer = File.CreateText(tokenFile))
            {
                var json = JsonSerializer.Serialize(token);
                await writer.WriteLineAsync(json);
            }
        }
    }
}
