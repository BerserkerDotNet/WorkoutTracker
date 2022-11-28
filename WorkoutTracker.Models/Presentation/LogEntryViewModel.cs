using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Models.Entities;

public class LogEntryViewModel
{
    public required Guid Id { get; set; }
    public required ExerciseViewModel Exercise { get; set; }
    public required DateTime Date { get; set; }
    public required IEnumerable<Set> Sets { get; set; }

    public double TotalDuration => Math.Ceiling(Sets.Sum(s => s.Duration.TotalMinutes));

    public double TotalRest => Math.Ceiling(Sets.Sum(s => s.RestTime.TotalMinutes));

    public double TotalWeightKG => Math.Ceiling(Sets.Sum(s => (s.WeightKG ?? 0) * s.Repetitions));

    public double TotalWeightLB => Math.Ceiling(Sets.Sum(s => (s.WeightLB ?? 0) * s.Repetitions));

    public static LogEntryViewModel New(WorkoutExerciseViewModel exercise) => new LogEntryViewModel
    {
        Id = Guid.NewGuid(),
        Exercise = new ExerciseViewModel { Id = exercise.Id, ImagePath = exercise.ImagePath, Name = exercise.Name },
        Date = DateTime.UtcNow,
        Sets = Enumerable.Empty<Set>()
    };
}

public record WorkoutExerciseSetViewModel(int Index, SetStatus Status, double Weight, int Reps, TimeSpan RestTime, TimeSpan Duration);

public record WorkoutExerciseViewModel(Guid Id, string Name, string ImagePath, IEnumerable<WorkoutExerciseSetViewModel> Sets);

public enum SetStatus
{
    NotStarted = 0,
    InProgress,
    Completed
}