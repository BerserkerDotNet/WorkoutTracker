using Microsoft.AspNetCore.Components.Forms;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.ViewModels;

public record ExercisesFilterViewModel(string Name, IEnumerable<string> MuscleGroups)
{
    public static ExercisesFilterViewModel Empty = new ExercisesFilterViewModel(string.Empty, Enumerable.Empty<string>());
}

public record ScheduleViewModel(Guid Id, int CurrentIndex, int TargetSets, TimeSpan TargetRest, IEnumerable<ExerciseViewModel> Exercises)
{
    public int TargetSets { get; set; } = TargetSets;

    public TimeSpan TargetRest { get; set; } = TargetRest;

    public ExerciseViewModel CurrentExercise => Exercises.Count() > CurrentIndex ? Exercises.ElementAt(CurrentIndex) : null;
}

public record WorkoutExerciseSetViewModel(int Index, SetStatus Status, double Weight, int Reps, TimeSpan RestTime, TimeSpan Duration);

public record WorkoutExerciseViewModel(Guid Id, string Name, string ImagePath, IEnumerable<WorkoutExerciseSetViewModel> Sets);

public record WorkoutViewModel(Guid Id, TimeSpan TargetRestTime, WorkoutExerciseViewModel Exercise);

public class EditExerciseViewModel : ExerciseViewModel
{
    public IBrowserFile ImageFile { get; set; }
}

public record WorkoutSet(int Index, SetStatus Status, double Weight, int Reps, TimeSpan RestTime, TimeSpan Duration)
{
    public int Reps { get; set; } = Reps;

    public double Weight { get; set; } = Weight;

    public static WorkoutSet CreateFromSet(int idx, Set set) => new WorkoutSet(idx, SetStatus.Completed, set.WeightLB.Value, set.Repetitions, set.RestTime, set.Duration);

    public static WorkoutSet CreateNewSet(int idx, SetStatus status, int weight = 0, int reps = 0) => new WorkoutSet(idx, status, weight, reps, TimeSpan.Zero, TimeSpan.Zero);
}

public enum SetStatus
{
    NotStarted = 0,
    InProgress,
    Completed
}