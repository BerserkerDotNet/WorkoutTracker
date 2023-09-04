using Mediator;
using Microsoft.Extensions.Logging;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;
using WorkoutTracker.Services.Statistics;

namespace WorkoutTracker.Services;

public record ServerData(IEnumerable<MuscleViewModel> Muscles,
    IEnumerable<ExerciseViewModel> Exercises,
    IEnumerable<LogEntryViewModel> Logs,
    IEnumerable<WorkoutProgram> Programs,
    Profile Profile);
    
public class DataSyncService
{
    private readonly IMediator _mediator;
    private readonly IWorkoutDataProvider _db;
    private readonly IWorkoutRepository _repository;
    private readonly ILogger<DataSyncService> _logger;

    public DataSyncService(IMediator mediator,  IWorkoutDataProvider db, IWorkoutRepository repository,  ILogger<DataSyncService> logger)
    {
        _mediator = mediator;
        _db = db;
        _repository = repository;
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
    
    public async Task UpdateStatistics()
    {
        var startDate = DateTime.UtcNow;
        var logs = _db.GetAllWorkoutLogs();
        var totalWorkoutStats = await _mediator.Send(new GetWorkoutsSummary(logs));
        var byMuscleGroup = await _mediator.Send(new GetPercentageByMuscleGroupStats(logs));
        var timeMetrics = await _mediator.Send(new GetWorkoutTimeMetrics(logs));

        _db.UpdateWorkoutStatistics(new WorkoutStatistics(totalWorkoutStats, timeMetrics, byMuscleGroup));
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