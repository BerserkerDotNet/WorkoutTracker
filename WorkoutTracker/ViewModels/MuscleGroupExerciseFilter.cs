public record MuscleGroupExerciseFilter(string GroupName) : IExerciseFilter
{
    public bool Match(ExerciseViewModel exercise)
    {
        return string.Equals(exercise.Muscles.First().MuscleGroup, GroupName, StringComparison.OrdinalIgnoreCase);
    }

    public static implicit operator MuscleGroupExerciseFilter(string group) => new MuscleGroupExerciseFilter(group);
}
