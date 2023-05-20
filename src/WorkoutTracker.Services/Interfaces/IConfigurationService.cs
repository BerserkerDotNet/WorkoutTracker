namespace WorkoutTracker.Services.Interfaces;

public record EndpointConfiguration(string Url, string Secret);

public interface IConfigurationService
{
    Task<EndpointConfiguration> GetEndpointConfigurationAsync();

    EndpointConfiguration GetEndpointConfiguration();

    Task SaveEndpointConfiguration(EndpointConfiguration config);

    bool IsConfigurationAvailable();
}