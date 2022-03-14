using BlazorState.Redux.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Reducers;
using WorkoutTracker.Data.Services;

namespace WorkoutTracker.Extensions;

public static class ServiceCollectionExtensions 
{
    public static void AddWorkoutTracker(this IServiceCollection services, Action<WorkoutTraclerConfigurator>? configure = null) 
    {
        var configuration = WorkoutTraclerConfigurator.Default;
        configure?.Invoke(configuration);

        var httpBuilder = services.AddHttpClient("api", (services, cfg) =>
        {
            cfg.BaseAddress = new Uri("https://workouttrackerfunctions.azurewebsites.net/api/");
        });

        if (configuration.MessageHandler is object)
        {
            httpBuilder.ConfigurePrimaryHttpMessageHandler(() => configuration.MessageHandler);
        }

        services.AddScoped<IWorkoutRepository, CachedWorkoutRepository>();
        services.AddSingleton(typeof(ICacheService), configuration.CacheService);
        services.AddSingleton(typeof(INotificationService), configuration.NotificationService);
        services.AddSingleton(typeof(IConfigurationService), configuration.ConfigurationService);
        services.AddReduxStore<RootState>(cfg =>
        {
            cfg.RegisterActionsFromAssemblyContaining<FetchExercisesAction>();
            cfg.Map<ExercisesReducer, ExercisesState>(s => s.Exercises);
            cfg.Map<ExerciseScheduleReducer, ExerciseScheduleState>(s => s.ExerciseSchedule);
            cfg.Map<LogEntriesReducer, LogEntriesState>(s => s.ExerciseLogs);
        });

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 2000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
    }
}

public class WorkoutTraclerConfigurator 
{
    public Type CacheService { get; private set; }

    public Type NotificationService { get; private set; }

    public Type ConfigurationService { get; private set; }

    public HttpMessageHandler MessageHandler { get; private set; }

    public string AuthenticationRedirectUrl { get; private set; }

    public WorkoutTraclerConfigurator WithCacheService<T>() 
        where T : ICacheService
    {
        CacheService = typeof(T);
        return this;
    }

    public WorkoutTraclerConfigurator WithNotificationService<T>()
         where T : INotificationService
    {
        NotificationService = typeof(T);
        return this;
    }

    public WorkoutTraclerConfigurator WithConfigurationService<T>()
         where T : IConfigurationService
    {
        ConfigurationService = typeof(T);
        return this;
    }

    public WorkoutTraclerConfigurator WithMessageHandler<T>()
        where T : HttpMessageHandler, new()
    {
        MessageHandler = new T();
        return this;
    }

    public WorkoutTraclerConfigurator WithAuthenticationRedirectUrl(string url)
    {
        AuthenticationRedirectUrl = url;
        return this;
    }

    public static WorkoutTraclerConfigurator Default => new WorkoutTraclerConfigurator()
        .WithCacheService<InMemoryCacheService>()
        .WithNotificationService<MudNotificationService>()
        .WithConfigurationService<NullConfigurationService>()
        .WithAuthenticationRedirectUrl("https://localhost:7210");
}