using System.Threading.Tasks;

namespace WorkoutTracker
{
    public record EndpointConfiguration(string Url, string Secret);

    public interface IConfigurationService
    {
        Task<EndpointConfiguration> GetEndpointConfigurationAsync();

        EndpointConfiguration GetEndpointConfiguration();

        Task SaveEndpointConfiguration(EndpointConfiguration config);

        bool IsConfigurationAvailable();
    }
}