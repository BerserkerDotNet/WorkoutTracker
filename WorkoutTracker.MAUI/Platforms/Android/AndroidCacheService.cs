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

        public void ResetExercisesCache()
        {
            File.Delete(exercisesFile);
        }

        public bool IsExercisesCached()
        {
            return File.Exists(exercisesFile);
        }
    }

    public class LocalConfigurationService : IConfigurationService
    {
        private string configFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "local.config.json");

        public EndpointConfiguration GetEndpointConfiguration()
        {
            if (!IsConfigurationAvailable())
            {
                throw new Exception("Configuration is missing.");
            }

            var json = File.ReadAllText(configFile);
            return JsonSerializer.Deserialize<EndpointConfiguration>(json);
        }

        public async Task<EndpointConfiguration> GetEndpointConfigurationAsync()
        {
            if (!IsConfigurationAvailable()) 
            {
                throw new Exception("Configuration is missing.");
            }

            var json = await File.ReadAllTextAsync(configFile);
            return JsonSerializer.Deserialize<EndpointConfiguration>(json);
        }

        public async Task SaveEndpointConfiguration(EndpointConfiguration config)
        {
            using (var writer = File.CreateText(configFile))
            {
                var json = JsonSerializer.Serialize(config);
                await writer.WriteLineAsync(json);
            }
        }

        public bool IsConfigurationAvailable() => File.Exists(configFile);
    }

    public interface IConfigurationService
    {
        Task<EndpointConfiguration> GetEndpointConfigurationAsync();

        EndpointConfiguration GetEndpointConfiguration();

        Task SaveEndpointConfiguration(EndpointConfiguration config);

        bool IsConfigurationAvailable();
    }

    public record EndpointConfiguration(string Url, string Secret);
}
