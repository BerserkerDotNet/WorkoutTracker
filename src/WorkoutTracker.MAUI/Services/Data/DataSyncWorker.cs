using Android.Content;
using Android.Util;
using AndroidX.Concurrent.Futures;
using AndroidX.Work;
using Google.Common.Util.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Services.Data.Entities;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;
using Xamarin.Android.Net;

namespace WorkoutTracker.MAUI.Services.Data;

public class DataSyncWorker : ListenableWorker, CallbackToFutureAdapter.IResolver
{
    public const string TAG = "DataSyncWorker";
    private static int NotificationId = 100;
    private readonly IWorkoutRepository _repository;

    public DataSyncWorker(Context context, WorkerParameters workerParams) :
        base(context, workerParams)
    {
        try
        {
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("WorkoutTracker.MAUI.appsettings.json");

            var config = new ConfigurationBuilder()
                        .AddJsonStream(stream)
                        .Build();

            var handler = new AuthenticatedClientHandler(new AuthenticationService(config))
            {
                InnerHandler = new AndroidMessageHandler()
            };
            var httpClient = new HttpClient(handler);
            httpClient.BaseAddress = new Uri(config["ApiEndpoint"]);
            _repository = new ApiRepositoryClient(httpClient, new ApplicationContext<ApiRepositoryClient>(new NullNotificationService(), new LoggerFactory().CreateLogger<ApiRepositoryClient>()));
        }
        catch (Exception ex)
        {
            Log.Error(TAG, $"Can't get workout repository service. {ex.Message}");
        }
    }

    public Java.Lang.Object AttachCompleter(CallbackToFutureAdapter.Completer callbackSignal)
    {
        Log.Debug(TAG, "Executing");
        Task.Run(async () => await SyncWithServer(callbackSignal));

        return TAG;
    }

    public override IListenableFuture StartWork()
    {
        Log.Debug(TAG, "Started.");
        return CallbackToFutureAdapter.GetFuture(this);
    }

    private async Task SyncWithServer(CallbackToFutureAdapter.Completer callbackSignal)
    {
        try
        {
            using var db = new WorkoutTrackerDb();
            var syncServiceLogger = new LoggerFactory().CreateLogger<DataSyncService>();
            var syncService = new DataSyncService(db, _repository, TimeProvider.System, syncServiceLogger);

            await syncService.SynchronizeData();
            syncService.UpdateStatistics();
            
            Log.Debug(TAG, "Sending notification.");
            var notification = new NotificationRequest
            {
                NotificationId = NotificationId++,
                Title = $"Workout data synchronized!",
            };
            await LocalNotificationCenter.Current.Show(notification);

            callbackSignal.Set(Result.InvokeSuccess());
        }
        catch (Exception ex)
        {
            Log.Error(TAG, ex.Message);
            var notification = new NotificationRequest
            {
                NotificationId = NotificationId++,
                Title = $"There was an error synchronizing data",
                Description = ex.Message
            };
            await LocalNotificationCenter.Current.Show(notification);
            callbackSignal.Set(Result.InvokeFailure());
        }
    }
}
