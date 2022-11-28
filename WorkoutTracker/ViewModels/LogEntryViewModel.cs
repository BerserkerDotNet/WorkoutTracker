using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.ViewModels;

public record WorkoutSummary(DateTime Date, WorkoutSetSummary Max, WorkoutSetSummary Min, WorkoutSetSummary Avg, WorkoutSetSummary Total, int SetsCount, Guid ExerciseId);

public record WorkoutSetSummary(double WeightKg, double WeightLb, int Repetitions, TimeSpan Duration, TimeSpan RestTime, IEnumerable<Set> Sets);

public record ExerciseIndicatorDescriptor(Guid Id, string Name, double Target, Func<WorkoutSetSummary, double> ValueSelector);