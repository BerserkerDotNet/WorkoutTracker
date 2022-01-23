using System.IO;
using System.Threading.Tasks;
using System.Text.Json;

namespace WorkoutTracker.MAUI.Android
{
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
}
