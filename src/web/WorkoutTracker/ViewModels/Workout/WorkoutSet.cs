namespace WorkoutTracker.ViewModels.Workout;

public class WorkoutSetTest
{
    public Guid Id { get; set; }

    public WorkoutSetState State { get; set; }

    public int Weight { get; set; }

    public int Repetitions { get; set; }

    public TimeSpan? RestTime { get; set; }

    public TimeSpan? WorkoutTime { get; set; }

    public DateTime? CompletionTime { get; set; }

}

public record SetsOverloadDecision(bool ShouldOverload, int NewWeight, int NewReps, IEnumerable<WorkoutSet> NewSets);
