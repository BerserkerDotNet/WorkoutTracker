namespace WorkoutTracker.ViewModels;

public record ExerciseIndicatorDescriptor(Guid Id, string Name, double Target, Func<WorkoutSetSummary, double> ValueSelector);