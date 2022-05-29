using BlazorApplicationInsights;
using Microsoft.Extensions.Logging;

namespace WorkoutTracker;

public class ApplicationContext : ILogger, INotificationService
{
    public ApplicationContext(INotificationService notifications, IApplicationInsights appInsights, ILogger logger)
    {
        Notifications = notifications;
        AppInsights = appInsights;
        Logger = logger;
    }

    public INotificationService Notifications { get; }

    public IApplicationInsights AppInsights { get; }

    public ILogger Logger { get; }

    public IDisposable BeginScope<TState>(TState state)
    {
        return Logger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return Logger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Logger.Log(logLevel, eventId, state, exception, formatter);
    }

    public Task TrackException(Error exception, string? id = null, SeverityLevel? severityLevel = null, Dictionary<string, object?>? properties = null) 
    {
        return AppInsights.TrackException(exception, id, severityLevel, properties);
    }

    public Task StartTrackEvent(string name) 
    {
        return AppInsights.StartTrackEvent(name);
    }

    public Task StopTrackEvent(string name, Dictionary<string, string?>? properties = null, Dictionary<string, decimal>? measurements = null) 
    {
        return AppInsights.StopTrackEvent(name, properties, measurements);
    }

    public Task Flush(bool? async = true) 
    {
        return AppInsights.Flush(async);
    }

    public void ShowToast(string message)
    {
        Notifications.ShowToast(message);
    }

    public void ShowError(string message)
    {
        Notifications.ShowError(message);
    }
}

public class ApplicationContext<TCategory> : ApplicationContext
{
    public ApplicationContext(INotificationService notifications, IApplicationInsights appInsights, ILogger<TCategory> logger)
        : base(notifications, appInsights, logger)
    {
    }
}