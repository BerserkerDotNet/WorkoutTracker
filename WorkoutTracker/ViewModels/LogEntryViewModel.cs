﻿using WorkoutTracker.Models;

namespace WorkoutTracker.ViewModels;

public record LogEntryViewModel(Guid Id, ExerciseViewModel Exercise, DateTime Date, IEnumerable<Set> Sets)
{
    public double TotalDuration => Math.Ceiling(Sets.Sum(s => s.Duration.TotalMinutes));

    public double TotalRest => Math.Ceiling(Sets.Sum(s => s.RestTime.TotalMinutes));

    public double TotalWeightKG => Math.Ceiling(Sets.Sum(s => (s.WeightKG ?? 0) * s.Repetitions));

    public double TotalWeightLB => Math.Ceiling(Sets.Sum(s => (s.WeightLB ?? 0) * s.Repetitions));

    public static LogEntryViewModel New(ExerciseViewModel exercise) => new LogEntryViewModel(Guid.NewGuid(), exercise, DateTime.UtcNow, Enumerable.Empty<Set>());
}

public record WorkoutSummary(DateTime Date, WorkoutSetSummary Max, WorkoutSetSummary Min, WorkoutSetSummary Avg, WorkoutSetSummary Total, int SetsCount, Guid ExerciseId);
public record WorkoutSetSummary(double WeightKg, double WeightLb, int Repetitions, TimeSpan Duration, TimeSpan RestTime);

public record ExerciseIndicatorDescriptor(Guid Id, string Name, double Target, Func<WorkoutSetSummary, double> ValueSelector);