using WorkoutTracker.Models;

namespace WorkoutTracker.ViewModels;

public class LogEntryViewModel
{
    public Guid Id { get; set; }

    public ExerciseViewModel Exercise { get; set; }

    public IEnumerable<Set> Sets { get; set; }

    public DateTime Date { get; set; }

    public double TotalDuration => Math.Ceiling(Sets.Sum(s => s.Duration.TotalMinutes));

    public double TotalRest => Math.Ceiling(Sets.Sum(s => s.RestTime.TotalMinutes));

    public double TotalWeightKG => Math.Ceiling(Sets.Sum(s => (s.WeightKG ?? 0) * s.Repetitions));

    public double TotalWeightLB => Math.Ceiling(Sets.Sum(s => (s.WeightLB ?? 0) * s.Repetitions));
}

public record WorkoutSummary(DateTime Date, WorkoutSetSummary Max, WorkoutSetSummary Min, WorkoutSetSummary Avg, WorkoutSetSummary Total, int SetsCount, Guid ExerciseId);
public record WorkoutSetSummary(double WeightKg, double WeightLb, int Repetitions, TimeSpan Duration, TimeSpan RestTime);

public record ExerciseIndicatorDescriptor(Guid Id, string Name, double Target, Func<WorkoutSetSummary, double> ValueSelector);