using Microsoft.Extensions.Logging;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.Services;

public record ServerData(IEnumerable<MuscleViewModel> Muscles,
    IEnumerable<ExerciseViewModel> Exercises,
    IEnumerable<LogEntryViewModel> Logs,
    IEnumerable<WorkoutProgram> Programs,
    Profile Profile);
    
public class DataSyncService
{
    private readonly IWorkoutDataProvider _db;
    private readonly IWorkoutRepository _repository;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<DataSyncService> _logger;

    public DataSyncService(IWorkoutDataProvider db, IWorkoutRepository repository, TimeProvider timeProvider,  ILogger<DataSyncService> logger)
    {
        _db = db;
        _repository = repository;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task SynchronizeData()
    {
        var changes = _db.GetPendingChanges();
        _logger.LogInformation($"There are {changes.Count()} items to sync with server.");

        foreach (var change in changes)
        {
            _logger.LogInformation($"Synchronizing table '{change.TableName}' with ID '{change.Id}'");
            Func<IWorkoutDataProvider, RecordToSyncViewModel, Task> updater = change.TableName switch
            {
                nameof(ExerciseViewModel) => UpdateExerciseRecord,
                nameof(LogEntryViewModel) => UpdateLogRecord,
                nameof(WorkoutProgram) => UpdateWorkoutProgram,
                nameof(Profile) => UpdateCurrentWorkout,
                _ => throw new NotSupportedException()
            };
            try
            {
                await updater(_db, change);
                _db.DeleteViewModel(change);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Failed to update record on the server. Table: {change.TableName}, ID: {change.Id}");
            }
        }
        _logger.LogInformation("Fetching data from server");
        var lastSyncDate = _db.GetLastSyncDate();
        var muscles = await _repository.GetMuscles();
        var exercises = await _repository.GetExercises();
        var logs = await _repository.GetLogs(lastSyncDate, DateTime.UtcNow);
        var programs = await _repository.GetWorkoutPrograms();
        var profile = await _repository.GetProfile();

        _logger.LogInformation("Updating database");
        _db.UpdateDataFromServer(new ServerData(muscles, exercises, logs, programs, profile));
        _logger.LogInformation("Update completed");
    }
    
    public void UpdateStatistics()
    {
        var today = _timeProvider.GetLocalNow().Date;
        
        int daysSinceMonday = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var beginningOfThisMonth = new DateTime(today.Year, today.Month, 1);
        var beginningOfThisWeek = today.AddDays(-daysSinceMonday);
        var logs = _db.GetWorkoutLogs(); 
        var totalCount = logs
            .GroupBy(e => new DateOnly(e.Date.Year, e.Date.Month, e.Date.Day))
            .Count();
        var thisMonth = logs
            .Where(e => e.Date >= beginningOfThisMonth)
            .GroupBy(e => new DateOnly(e.Date.Year, e.Date.Month, e.Date.Day))
            .Count();
        
        var thisWeek = logs
            .Where(e => e.Date >= beginningOfThisWeek)
            .GroupBy(e => new DateOnly(e.Date.Year, e.Date.Month, e.Date.Day))
            .Count();

        _db.UpdateWorkoutStatistics(new TotalWorkoutData(totalCount, thisWeek, thisMonth));
    }

    private async Task UpdateExerciseRecord(IWorkoutDataProvider db, RecordToSyncViewModel record)
    {
        if (record.OpType == OperationType.Delete)
        {
            await _repository.DeleteExercise(record.RecordId);
        }
        else
        {
            var exerciseToSync = db.GetViewModel<ExerciseViewModel>(record.RecordId);
            if (exerciseToSync is not null)
            {
                await _repository.UpdateExercise(exerciseToSync);
            }
        }
    }

    private async Task UpdateWorkoutProgram(IWorkoutDataProvider db, RecordToSyncViewModel record)
    {
        if (record.OpType == OperationType.Delete)
        {
            await _repository.DeleteWorkoutProgram(record.RecordId);
        }
        else
        {
            var programToSync = db.GetViewModel<WorkoutProgram>(record.RecordId);
            if (programToSync is not null)
            {
                await _repository.UpdateProgram(programToSync);
            }
        }
    }

    private async Task UpdateCurrentWorkout(IWorkoutDataProvider db, RecordToSyncViewModel record)
    {
        var profile = db.GetViewModel<Profile>(record.RecordId);
        if (profile is not null && profile.CurrentWorkout.HasValue)
        {
            await _repository.SetCurrentWorkoutProgram(profile.CurrentWorkout.Value);
        }
    }

    private async Task UpdateLogRecord(IWorkoutDataProvider db, RecordToSyncViewModel record)
    {
        if (record.OpType == OperationType.Delete)
        {
            await _repository.DeleteLog(record.RecordId);
        }
        else
        {
            var logToSync = db.GetViewModel<LogEntryViewModel>(record.RecordId);
            if (logToSync is not null)
            {
                await _repository.AddLogRecord(logToSync);
            }
        }
    }
}