using Microsoft.Extensions.Logging;
using System;
using WorkoutTracker.MAUI.Interfaces;

namespace WorkoutTracker.Services;

public class ApplicationContext : ILogger, INotificationService
{
    public ApplicationContext(INotificationService notifications, ILogger logger)
    {
        Notifications = notifications;
        Logger = logger;
    }

    public INotificationService Notifications { get; }

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
    public ApplicationContext(INotificationService notifications, ILogger<TCategory> logger)
        : base(notifications, logger)
    {
    }
}
