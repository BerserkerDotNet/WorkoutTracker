namespace WorkoutTracker.Data.Services;

public class NullConfigurationService : IConfigurationService
{
    private EndpointConfiguration _configuration;

    public EndpointConfiguration GetEndpointConfiguration()
    {
        return _configuration;
    }

    public Task<EndpointConfiguration> GetEndpointConfigurationAsync()
    {
        return Task.FromResult(_configuration);
    }

    public bool IsConfigurationAvailable()
    {
        return _configuration is object;
    }

    public Task SaveEndpointConfiguration(EndpointConfiguration config)
    {
        _configuration = config;
        return Task.CompletedTask;
    }
}
