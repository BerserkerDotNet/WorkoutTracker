using Android.Content;
using Android.Util;
using AndroidX.Concurrent.Futures;
using AndroidX.Work;
using Google.Common.Util.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Plugin.LocalNotification;
using System;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Extensions;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using Xamarin.Android.Net;

namespace WorkoutTracker.MAUI.Services.Data;

public class DataSyncWorker : ListenableWorker, CallbackToFutureAdapter.IResolver
{
    public const string TAG = "DataSyncWorker";
    private static int NotificationId = 100;
    private readonly DataSyncService _dataSyncService;

    public DataSyncWorker(Context context, WorkerParameters workerParams) :
        base(context, workerParams)
    {
        try
        {
            var builder = Host.CreateApplicationBuilder();
            builder.Configuration.AddConfig();
            
            builder.Services.AddWorkoutTracker();
            
            var httpClientBuilder = builder.Services.AddHttpClient<IWorkoutRepository, ApiRepositoryClient>((client, sp) =>
                {
                    client.BaseAddress = new Uri(builder.Configuration["ApiEndpoint"]);
                    return new ApiRepositoryClient(client, sp.GetRequiredService<ApplicationContext<ApiRepositoryClient>>());
            })
            .AddHttpMessageHandler<AuthenticatedClientHandler>()
            .ConfigurePrimaryHttpMessageHandler<AndroidMessageHandler>();
            var app = builder.Build();
            _dataSyncService = app.Services.GetService<DataSyncService>();
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
            await _dataSyncService.SynchronizeData();
            await _dataSyncService.UpdateStatistics();
            
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
