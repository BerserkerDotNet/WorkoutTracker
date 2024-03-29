﻿using Microsoft.Maui.Storage;
using SQLite;
using SQLiteNetExtensions.Extensions;
using SQLiteNetExtensions.Extensions.TextBlob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using WorkoutTracker.MAUI.Services.Data.Entities;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.MAUI.Services.Data;

public sealed class SystemTextJsonBlobSerializer : ITextBlobSerializer
{
    private static readonly JsonSerializerOptions _options;

    static SystemTextJsonBlobSerializer()
    {
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _options.Converters.Add(new IExerciseSetPolymorphicJsonConverter());
    }

    public object Deserialize(string text, Type type)
    {
        return JsonSerializer.Deserialize(text, type, _options);
    }

    public string Serialize(object element)
    {
        return JsonSerializer.Serialize(element, _options);
    }
}

public class WorkoutTrackerDb : IWorkoutDataProvider, IDisposable
{
    private const string DatabaseFilename = "WorkoutTrackerDb.db3";
    private static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);
    private SQLiteConnection _database;

    public WorkoutTrackerDb()
    {
        Init();
    }

    private void Init()
    {
        if (_database is not null)
            return;

        TextBlobOperations.SetTextSerializer(new SystemTextJsonBlobSerializer());
        _database = new SQLiteConnection(DatabasePath,
            SQLiteOpenFlags.ReadWrite |
            SQLiteOpenFlags.Create |
            SQLiteOpenFlags.SharedCache);
        _database.CreateTable<MuscleDbEntity>();
        _database.CreateTable<ExerciseDbEntity>();
        _database.CreateTable<MusclesToExercises>();
        _database.CreateTable<LogsDbEntity>();
        _database.CreateTable<ProgramsDbEntity>();
        _database.CreateTable<Settings>();
        _database.CreateTable<WorkoutStatisticsEntity>();
        _database.CreateTable<RecordsToSync>();
        _database.CreateTable<ProfileDbEntity>();
    }

    public void UpdateDataFromServer(ServerData data)
    {
        var muscles = data.Muscles.Select(MuscleDbEntity.FromViewModel);
        var exercises = data.Exercises.Select(ExerciseDbEntity.FromViewModel);
        var logs = data.Logs.Select(LogsDbEntity.FromViewModel);
        var programs = data.Programs.Select(ProgramsDbEntity.FromViewModel);
        var profile = ProfileDbEntity.FromViewModel(data.Profile);

        _database.RunInTransaction(() =>
        {
            _database.InsertOrReplaceAllWithChildren(muscles);
            _database.InsertOrReplaceAllWithChildren(exercises);
            _database.InsertOrReplaceAllWithChildren(logs);
            _database.InsertOrReplaceAllWithChildren(programs);
            _database.InsertOrReplaceWithChildren(profile);
            var settings = _database.Table<Settings>().FirstOrDefault() ?? new Settings { Id = Guid.NewGuid() };
            settings.LastSyncDate = DateTime.UtcNow;
            _database.Update(settings);
        });
    }

    public IEnumerable<RecordToSyncViewModel> GetPendingChanges()
    {
        return _database.Table<RecordsToSync>()
            .Select(r => r.ToViewModel())
            .ToList();
    }

    public IExerciseSet GetMaxWeightLiftedOnExercise(Guid id)
    {
        var logs = _database.GetAllWithChildren<LogsDbEntity>(e => e.ExerciseId == id)
            .ToArray();

        var legacyLogs = logs.SelectMany(e => e.Sets)
            .OfType<LegacySet>()
            .ToArray();

        var maxSet = legacyLogs.MaxBy(s => s.WeightLB);

        return maxSet;
    }

    public LogEntryViewModel GetLastEntryForExercise(Guid id)
    {
        var log = _database.GetAllWithChildren<LogsDbEntity>(l => l.ExerciseId == id, recursive: true)
            .OrderByDescending(l => l.Date)
            .Where(l => l.Sets.Any(s => s is LegacySet || s is CompletedSet))
            .FirstOrDefault();

        if (log is null)
        {
            return null;
        }

        return log.ToViewModel();
    }

    public DateTime GetLastSyncDate()
    {
        var settings = _database.Table<Settings>().FirstOrDefault();
        return settings?.LastSyncDate ?? DateTime.MinValue;
    }

    public WorkoutStatistics GetWorkoutStatistics()
    {
        var stats = _database.GetAllWithChildren<WorkoutStatisticsEntity>().FirstOrDefault();
        if (stats is null || stats.Summary is null || stats.PercentageByMuscleGroup is null)
        {
            return new WorkoutStatistics(WorkoutsSummary.Empty, WorkoutTimeMetrics.Empty,
                Enumerable.Empty<DataSeriesItem>());
        }

        return new WorkoutStatistics(stats.Summary, stats.TimeMetrics, stats.PercentageByMuscleGroup);
    }

    public void UpdateWorkoutStatistics(WorkoutStatistics data)
    {
        var stats = _database.Table<WorkoutStatisticsEntity>().FirstOrDefault() ??
                    new WorkoutStatisticsEntity { Id = Guid.NewGuid() };

        stats.Summary = data.Summary;
        stats.TimeMetrics = data.TimeMetrics;
        stats.PercentageByMuscleGroup = data.PercentagePerMuscleGroup;

        _database.InsertOrReplaceWithChildren(stats);
    }

    public IEnumerable<MuscleViewModel> GetMuscles()
    {
        var data = _database.GetAllWithChildren<MuscleDbEntity>(recursive: true);
        return data.Select(e => e.ToViewModel()).ToArray();
    }

    public IEnumerable<ExerciseViewModel> GetExercises()
    {
        var data = _database.GetAllWithChildren<ExerciseDbEntity>(recursive: true);
        return data.Select(e => e.ToViewModel()).ToArray();
    }

    public IEnumerable<WorkoutProgram> GetPrograms()
    {
        var data = _database.GetAllWithChildren<ProgramsDbEntity>(recursive: true);
        return data.Select(e => e.ToViewModel()).ToArray();
    }

    public Profile GetProfile()
    {
        var profile = _database.Table<ProfileDbEntity>().FirstOrDefault();
        if (profile is null)
        {
            return new Profile { Name = "N/A" };
        }

        return profile.ToViewModel();
    }

    public void SetCurrentWorkout(Guid id)
    {
        var profile = GetProfile();
        profile.CurrentWorkout = id;
        UpdateViewModel(profile);
    }

    public IEnumerable<LogEntryViewModel> GetWorkoutLogs(DateTime startDate, int daysToFetch)
    {
        var endDate = startDate.AddDays(-daysToFetch);
        var items = _database.CreateCommand($"SELECT * from '{nameof(LogsDbEntity)}' where Date > {endDate.Ticks}")
            .ExecuteQuery<LogsDbEntity>();

        foreach (var item in items)
        {
            _database.GetChildren(item, true);
        }

        return items.OrderByDescending(e => e.Date)
            .ThenBy(e=> e.Order)
            .Select(e => e.ToViewModel())
            .ToArray();
    }
    
    public IEnumerable<LogEntryViewModel> GetAllWorkoutLogs()
    {
        return _database.GetAllWithChildren<LogsDbEntity>(recursive: true)
            .OrderByDescending(e => e.Date)
            .Select(e => e.ToViewModel())
            .ToArray();
    }

    public T Get<T>(Guid id)
        where T : BaseDbEntity, new()
    {
        return _database.GetWithChildren<T>(id, recursive: true);
    }

    public IEnumerable<LogEntryViewModel> GetTodaysSchedule()
    {
        var today = DateTime.Today.ToUniversalTime();
        return _database.GetAllWithChildren<LogsDbEntity>(e => e.Date == today, recursive: true)
            .OrderBy(e => e.Order)
            .Select(e => e.ToViewModel());
    }

    public void Delete<T>(T recordToDelete)
        where T : BaseDbEntity
    {
        _database.Delete(recordToDelete);
    }

    public T GetViewModel<T>(Guid recordId)
        where T : class
    {
        var targetType = typeof(T);
        if (targetType == typeof(ExerciseViewModel))
        {
            return _database.GetWithChildren<ExerciseDbEntity>(recordId, recursive: true).ToViewModel() as T;
        }

        if (targetType == typeof(LogEntryViewModel))
        {
            return _database.GetWithChildren<LogsDbEntity>(recordId, recursive: true).ToViewModel() as T;
        }

        if (targetType == typeof(WorkoutProgram))
        {
            return _database.GetWithChildren<ProgramsDbEntity>(recordId, recursive: true).ToViewModel() as T;
        }

        if (targetType == typeof(RecordToSyncViewModel))
        {
            return _database.GetWithChildren<RecordsToSync>(recordId, recursive: true).ToViewModel() as T;
        }

        throw new NotImplementedException($"Get of {typeof(T)} entity is not implemented.");
    }

    public void DeleteViewModel<T>(T model)
    {
        BaseDbEntity dbData = model switch
        {
            ExerciseViewModel eVm => ExerciseDbEntity.FromViewModel(eVm),
            LogEntryViewModel lVm => LogsDbEntity.FromViewModel(lVm),
            WorkoutProgram lVm => ProgramsDbEntity.FromViewModel(lVm),
            RecordToSyncViewModel rVm => RecordsToSync.FromViewModel(rVm),
            _ => throw new NotImplementedException($"Delete of {typeof(T)} entity is not implemented.")
        };

        _database.Delete(dbData);
        _database.InsertOrReplace(new RecordsToSync
        {
            Id = dbData.Id, RecordId = dbData.Id, TableName = model.GetType().Name, OpType = OperationType.Delete
        });
    }

    public void UpdateViewModel<T>(T model)
    {
        BaseDbEntity dbData = model switch
        {
            ExerciseViewModel eVm => ExerciseDbEntity.FromViewModel(eVm),
            LogEntryViewModel lVm => LogsDbEntity.FromViewModel(lVm),
            WorkoutProgram lVm => ProgramsDbEntity.FromViewModel(lVm),
            Profile lvm => ProfileDbEntity.FromViewModel(lvm),
            _ => throw new NotImplementedException($"Update to {typeof(T)} entity is not implemented.")
        };

        _database.InsertOrReplaceWithChildren(dbData, recursive: true);
        _database.InsertOrReplace(new RecordsToSync
        {
            Id = dbData.Id, RecordId = dbData.Id, TableName = model.GetType().Name, OpType = OperationType.Update
        });
    }

    public void Dispose()
    {
        _database.Dispose();
    }
}