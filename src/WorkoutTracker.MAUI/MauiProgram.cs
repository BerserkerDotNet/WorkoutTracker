﻿using AndroidX.Work;
using CommunityToolkit.Maui;
using DevExpress.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Plugin.LocalNotification;
using System;
using System.Reflection;
using WorkoutTracker.MAUI.Android;
using WorkoutTracker.MAUI.Extensions;
using WorkoutTracker.MAUI.Services;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using Xamarin.Android.Net;
using INavigation = WorkoutTracker.Services.Interfaces.INavigation;
using INotificationService = WorkoutTracker.Services.Interfaces.INotificationService;

namespace WorkoutTracker.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureEssentials()
            .UseDevExpress(useLocalization: true)
            .UseMauiCommunityToolkit()
            .AddViewModels()
            .AddViews()
            .UseLocalNotification()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("roboto-regular.ttf", "Roboto");
                fonts.AddFont("roboto-medium.ttf", "Roboto-Medium");
                fonts.AddFont("roboto-bold.ttf", "Roboto-Bold");
                fonts.AddFont("univia-pro-regular.ttf", "Univia-Pro");
                fonts.AddFont("univia-pro-medium.ttf", "Univia-Pro Medium");
            });

        builder.Configuration.AddConfig();
        
#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddWorkoutTracker();

        //var httpClientBuilder = builder.Services.AddHttpClient<IWorkoutRepository, CachedWorkoutRepositoryDecorator>((client, sp) =>
        //{
        //    client.BaseAddress = new Uri(config["ApiEndpoint"]);

        //    var apiRepo = new ApiRepositoryClient(client, sp.GetRequiredService<ApplicationContext<ApiRepositoryClient>>());
        //    return new CachedWorkoutRepositoryDecorator(apiRepo, sp.GetRequiredService<ICacheService>(), sp.GetRequiredService<ApplicationContext<CachedWorkoutRepositoryDecorator>>());
        //})
        //.AddHttpMessageHandler<AuthenticatedClientHandler>()
        //.ConfigurePrimaryHttpMessageHandler<AndroidMessageHandler>();
        
        using var workerConstraintsbuilder = new Constraints.Builder();
        workerConstraintsbuilder.SetRequiredNetworkType(NetworkType.Connected);
        var workConstraints = workerConstraintsbuilder.Build();

        var workerRequest = new PeriodicWorkRequest.Builder(typeof(DataSyncWorker), TimeSpan.FromDays(1))
            .SetConstraints(workConstraints)
            .AddTag(DataSyncWorker.TAG)
            .Build();

        WorkManager.GetInstance(Microsoft.Maui.ApplicationModel.Platform.AppContext)
            .EnqueueUniquePeriodicWork(DataSyncWorker.TAG, ExistingPeriodicWorkPolicy.Keep, workerRequest);

        return builder.Build();
    }
}