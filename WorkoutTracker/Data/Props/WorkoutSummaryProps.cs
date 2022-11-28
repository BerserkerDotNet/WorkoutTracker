namespace WorkoutTracker.Data.Props;

public class WorkoutSummaryProps
{
    public IEnumerable<WorkoutSummary> Summaries { get; set; }

    public IEnumerable<ExerciseViewModel> Exercises { get; set; }
}
