﻿using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.MAUI.Interfaces;

public interface IWorkoutRepository
{
    Task<IEnumerable<MuscleViewModel>> GetMuscles();

    Task<IEnumerable<ExerciseViewModel>> GetExercises();

    Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime date);

    Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime from, DateTime to);

    Task<LogEntryViewModel> GetPreviousWorkoutStatsBy(Guid exerciseId);

    Task<IEnumerable<WorkoutSummary>> GetWorkoutSummaries(DateTime from, DateTime to);

    Task<IEnumerable<WorkoutProgram>> GetWorkoutPrograms();

    Task DeleteWorkoutProgram(Guid id);

    Task AddLogRecord(LogEntryViewModel model);

    Task DeleteLog(Guid id);

    Task DeleteExercise(Guid id);

    Task UpdateExercise(ExerciseViewModel exercise);

    Task UpdateMuscle(MuscleViewModel muscle);

    Task UpdateProgram(WorkoutProgram program);

    Task<bool> UploadImage(FileResult file, string imagePath);
}