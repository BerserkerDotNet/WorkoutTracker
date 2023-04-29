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
using WorkoutTracker.MAUI.Interfaces;
using WorkoutTracker.MAUI.Services.Data.Entities;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using Xamarin.Android.Net;

namespace WorkoutTracker.MAUI.Services.Data;

public record ServerData(IEnumerable<MuscleViewModel> Muscles,
    IEnumerable<ExerciseViewModel> Exercises,
    IEnumerable<LogEntryViewModel> Logs,
    IEnumerable<WorkoutProgram> Programs,
    Profile Profile);

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
            var changes = db.GetPendingChanges();
            Log.Debug(TAG, $"There are {changes.Count()} items to sync with server.");

            foreach (var change in changes)
            {
                Log.Debug(TAG, $"Synchronizing table '{change.TableName}' with ID '{change.Id}'");
                Func<WorkoutTrackerDb, RecordsToSync, Task> updater = change.TableName switch
                {
                    nameof(ExerciseViewModel) => UpdateExerciseRecord,
                    nameof(LogEntryViewModel) => UpdateLogRecord,
                    nameof(WorkoutProgram) => UpdateWorkoutProgram,
                    nameof(Profile) => UpdateCurrentWorkout,
                    _ => throw new NotSupportedException()
                };
                try
                {
                    await updater(db, change);
                    db.Delete(change);
                }
                catch (Exception ex)
                {
                    Log.Error(TAG, Java.Lang.Throwable.FromException(ex), $"Failed to update record on the server. Table: {change.TableName}, ID: {change.Id}");
                }
            }

            Log.Debug(TAG, "Fetching data from server.");
            var lastSyncDate = db.GetLastSyncDate();
            var muscles = await _repository.GetMuscles();
            var exercises = await _repository.GetExercises();
            var logs = await _repository.GetLogs(lastSyncDate, DateTime.UtcNow);
            var programs = await _repository.GetWorkoutPrograms();
            var profile = await _repository.GetProfile();

            Log.Debug(TAG, "Updating database.");
            db.UpdateDataFromServer(new ServerData(muscles, exercises, logs, programs, profile));

            Log.Debug(TAG, "Sending notification.");
            var notification = new NotificationRequest
            {
                NotificationId = NotificationId++,
                Title = $"Workout data synchronized!",
            };
            LocalNotificationCenter.Current.Show(notification).GetAwaiter().GetResult();

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
            LocalNotificationCenter.Current.Show(notification).GetAwaiter().GetResult();
            callbackSignal.Set(Result.InvokeFailure());
        }
    }

    private async Task UpdateExerciseRecord(WorkoutTrackerDb db, RecordsToSync record)
    {
        if (record.OpType == OperationType.Delete)
        {
            await _repository.DeleteExercise(record.RecordId);
        }
        else
        {
            var exerciseToSync = db.Get<ExerciseDbEntity>(record.RecordId);
            if (exerciseToSync is not null)
            {
                await _repository.UpdateExercise(exerciseToSync.ToViewModel());
            }
        }
    }

    private async Task UpdateWorkoutProgram(WorkoutTrackerDb db, RecordsToSync record)
    {
        if (record.OpType == OperationType.Delete)
        {
            await _repository.DeleteWorkoutProgram(record.RecordId);
        }
        else
        {
            var programToSync = db.Get<ProgramsDbEntity>(record.RecordId);
            if (programToSync is not null)
            {
                await _repository.UpdateProgram(programToSync.ToViewModel());
            }
        }
    }

    private async Task UpdateCurrentWorkout(WorkoutTrackerDb db, RecordsToSync record)
    {
        var profile = db.Get<ProfileDbEntity>(record.RecordId);
        if (profile is not null && profile.CurrentWorkout.HasValue)
        {
            await _repository.SetCurrentWorkoutProgram(profile.CurrentWorkout.Value);
        }
    }

    private async Task UpdateLogRecord(WorkoutTrackerDb db, RecordsToSync record)
    {
        if (record.OpType == OperationType.Delete)
        {
            await _repository.DeleteLog(record.RecordId);
        }
        else
        {
            var logToSync = db.Get<LogsDbEntity>(record.RecordId);
            if (logToSync is not null)
            {
                await _repository.AddLogRecord(logToSync.ToViewModel());
            }
        }
    }
}
