namespace WorkoutTracker.ViewModels.Workout;

public class WorkoutSchedule
{
    public Guid Id { get; set; }

    public IList<WorkoutExercise> Exercises { get; set; }
}

public class WorkoutProfile
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public IEnumerable<IExerciseSelector> ExerciseSelectors { get; set; }
}

public class WeeklySchedule
{
    public WorkoutProfile Monday { get; set; }

    public WorkoutProfile Tuesday { get; set; }

    public WorkoutProfile Wednesday { get; set; }

    public WorkoutProfile Thursday { get; set; }

    public WorkoutProfile Friday { get; set; }

    public WorkoutProfile Saturday { get; set; }

    public WorkoutProfile Sunday { get; set; }
}