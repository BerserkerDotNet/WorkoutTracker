using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using System;
using System.Reflection;
using WorkoutTracker.MAUI.Android;
using WorkoutTracker.MAUI.Services;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.MAUI.ViewModels;
using WorkoutTracker.MAUI.Views;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.ViewModels;
using Xamarin.Android.Net;
using INotificationService = WorkoutTracker.Services.Interfaces.INotificationService;
using INavigation = WorkoutTracker.Services.Interfaces.INavigation;

namespace WorkoutTracker.MAUI.Extensions;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder AddViewModels(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<WorkoutViewModel>();
        builder.Services.AddSingleton<WorkoutStatsViewModel>();
        builder.Services.AddSingleton<WorkoutProgramsViewModel>();
        builder.Services.AddSingleton<LibraryViewModel>();
        builder.Services.AddSingleton<EditExercisePageViewModel>();
        builder.Services.AddSingleton<EditWorkoutProgramViewModel>();
        builder.Services.AddSingleton<EditWorkoutDefinitionViewModel>();
        return builder;
    }

    public static MauiAppBuilder AddViews(this MauiAppBuilder builder)
    {
        builder.Services.AddSingleton<Workout>();
        builder.Services.AddSingleton<WorkoutStats>();
        builder.Services.AddSingleton<Library>();
        builder.Services.AddSingleton<WorkoutPrograms>();
        builder.Services.AddSingleton<EditExercisePage>();
        builder.Services.AddSingleton<EditWorkoutProgram>();
        builder.Services.AddSingleton<EditWorkoutDefinition>();
        return builder;
    }

    public static void AddConfig(this ConfigurationManager manager)
    {
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("WorkoutTracker.MAUI.appsettings.json");

        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
        manager.AddConfiguration(config);
    }

    public static IServiceCollection AddWorkoutTracker(this IServiceCollection services)
    {
        services.AddMediator();
        services.AddScoped(typeof(ApplicationContext<>));
        services.AddSingleton(new CDNImageProvider(new Uri("https://workout-tracker-content.azureedge.net/images/")));
        services.AddScoped<AuthenticationService>();
        services.AddScoped<ICacheService, AndroidCacheService>();
        services.AddScoped<INotificationService, CommunityToolkitNotificationService>();
        services.AddScoped<AndroidMessageHandler>();
        services.AddScoped<AuthenticatedClientHandler>();
        services.AddSingleton<WorkoutTrackerDb>();
        services.AddSingleton<IWorkoutDataProvider, WorkoutTrackerDb>();
        services.AddSingleton<INavigation, ShellNavigation>();
        services.AddSingleton<ISetsGenerator, SetsGenerator>();
        services.AddSingleton<IExerciseTimerService, ExerciseTimerService>();
        services.AddSingleton<DataSyncService>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}