using BlazorApplicationInsights;
using BlazorState.Redux.Extensions;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Reducers;
using WorkoutTracker.Data.Services;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddWorkoutTracker(this IServiceCollection services, IConfiguration configuration, Action<WorkoutTrackerConfigurator>? configurator = null)
    {
        ConfigureMappingRules();

        var trackerServicesConfig = WorkoutTrackerConfigurator.Default;
        configurator?.Invoke(trackerServicesConfig);

        var httpBuilder = services.AddHttpClient("api", (services, cfg) =>
        {
            cfg.BaseAddress = new Uri(configuration["ApiEndpoint"]);
        });

        if (trackerServicesConfig.MessageHandler is object)
        {
            httpBuilder.ConfigurePrimaryHttpMessageHandler(() => trackerServicesConfig.MessageHandler);
        }

        services.AddBlazorApplicationInsights(async applicationInsights =>
        {
            var telemetryItem = new TelemetryItem()
            {
                Tags = new Dictionary<string, object>()
                {
                    { "ai.cloud.role", configuration["AppName"] },
                    { "ai.cloud.roleInstance", "FrontEnd App" },
                }
            };

            await applicationInsights.AddTelemetryInitializer(telemetryItem);
            await applicationInsights.TrackPageView();
        });

        services.AddSingleton(new CDNImageProvider(new Uri("https://workout-tracker-content.azureedge.net/images/")));
        services.AddSingleton<IExerciseTimerService, ExerciseTimerService>();
        services.AddScoped<IPropsConfig, PropsConfigurations>();
        services.AddScoped<PropsProvider>();
        services.AddScoped(typeof(ApplicationContext<>));
        services.AddScoped<WorkoutSetsService>();
        services.AddScoped<IWorkoutRepository, CachedWorkoutRepository>();
        services.AddScoped(typeof(ICacheService), trackerServicesConfig.CacheService);
        services.AddScoped(typeof(INotificationService), trackerServicesConfig.NotificationService);
        services.AddScoped(typeof(IConfigurationService), trackerServicesConfig.ConfigurationService);
        services.AddReduxStore<RootState>(cfg =>
        {
            cfg.RegisterActionsFromAssemblyContaining<FetchExercisesAction>();
            cfg.Map<ExercisesReducer, ExercisesState>(s => s.Exercises);
            cfg.Map<ExerciseScheduleReducer, ExerciseScheduleState>(s => s.ExerciseSchedule);
            cfg.Map<LogEntriesReducer, LogEntriesState>(s => s.ExerciseLogs);
            cfg.Map<UIReducer, UIState>(s => s.UI);
            cfg.Map<UserPreferencesReducer, UserPreferencesState>(s => s.Preferences);
        });

        services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 2000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
        });
    }

    private static void ConfigureMappingRules()
    {
        TypeAdapterConfig<ExerciseViewModel, Exercise>
            .ForType()
            .Map(dest => dest.Muscles, src => src.Muscles.Select(m => m.Id).ToArray());
    }
}

public class WorkoutTrackerConfigurator
{
    public Type CacheService { get; private set; }

    public Type NotificationService { get; private set; }

    public Type ConfigurationService { get; private set; }

    public HttpMessageHandler MessageHandler { get; private set; }

    public string AuthenticationRedirectUrl { get; private set; }

    public WorkoutTrackerConfigurator WithCacheService<T>()
        where T : ICacheService
    {
        CacheService = typeof(T);
        return this;
    }

    public WorkoutTrackerConfigurator WithNotificationService<T>()
         where T : INotificationService
    {
        NotificationService = typeof(T);
        return this;
    }

    public WorkoutTrackerConfigurator WithConfigurationService<T>()
         where T : IConfigurationService
    {
        ConfigurationService = typeof(T);
        return this;
    }

    public WorkoutTrackerConfigurator WithMessageHandler<T>()
        where T : HttpMessageHandler, new()
    {
        MessageHandler = new T();
        return this;
    }

    public WorkoutTrackerConfigurator WithAuthenticationRedirectUrl(string url)
    {
        AuthenticationRedirectUrl = url;
        return this;
    }

    public static WorkoutTrackerConfigurator Default => new WorkoutTrackerConfigurator()
        .WithCacheService<InMemoryCacheService>()
        .WithNotificationService<MudNotificationService>()
        .WithConfigurationService<NullConfigurationService>()
        .WithAuthenticationRedirectUrl("https://localhost:7210");
}