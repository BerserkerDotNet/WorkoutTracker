using System;
using System.Collections.Generic;
using WorkoutTracker.Models.Contracts;

namespace WorkoutTracker.Models.Presentation;

public class LogEntryViewModel
{
    public required Guid Id { get; set; }
    public int Order { get; set; }
    public required ExerciseViewModel Exercise { get; set; }
    public required DateTime Date { get; set; }
    public required IEnumerable<IExerciseSet> Sets { get; set; }
}

public record WorkoutSummary(DateTime Date, WorkoutSetSummary Max, WorkoutSetSummary Min, WorkoutSetSummary Avg, WorkoutSetSummary Total, int SetsCount, Guid ExerciseId);

public record WorkoutSetSummary(double WeightKg, double WeightLb, int Repetitions, TimeSpan Duration, TimeSpan RestTime, IEnumerable<IExerciseSet> Sets);

public record WorkoutExerciseSetViewModel(int Index, SetStatus Status, double Weight, int Reps, TimeSpan RestTime, TimeSpan Duration);

public record WorkoutExerciseViewModel(Guid Id, string Name, string ImagePath, IEnumerable<WorkoutExerciseSetViewModel> Sets);

public enum SetStatus
{
    NotStarted = 0,
    InProgress,
    Completed
}