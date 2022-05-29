public interface IExerciseSelector
{
    ExerciseDescriptor Select(IEnumerable<ExerciseViewModel> exercises);
}

public record ExerciseDescriptor(IEnumerable<ExerciseViewModel> MatchedExercises, int? TargetSets = null, TimeSpan? TargetRestTime = null);