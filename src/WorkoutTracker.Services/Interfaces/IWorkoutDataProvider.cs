﻿using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.Services.Interfaces;

public interface IWorkoutDataProvider
{
    DateTime GetLastSyncDate();

    void UpdateDataFromServer(ServerData data);
    
    IExerciseSet GetMaxWeightLiftedOnExercise(Guid id);

    LogEntryViewModel GetLastEntryForExercise(Guid id);

    IEnumerable<MuscleViewModel> GetMuscles();

    IEnumerable<ExerciseViewModel> GetExercises();
    
    IEnumerable<RecordToSyncViewModel> GetPendingChanges();
    
    void UpdateViewModel<T>(T model);
    
    void DeleteViewModel<T>(T model);

    void SetCurrentWorkout(Guid id);

    Profile GetProfile();

    IEnumerable<WorkoutProgram> GetPrograms();

    IEnumerable<LogEntryViewModel> GetTodaysSchedule();
    
    IEnumerable<LogEntryViewModel> GetWorkoutLogs(DateTime startDate, int daysToFetch);

    IEnumerable<LogEntryViewModel> GetAllWorkoutLogs();

    WorkoutStatistics GetWorkoutStatistics();
    
    void UpdateWorkoutStatistics(WorkoutStatistics data);

    T GetViewModel<T>(Guid recordId)
        where T : class;
}