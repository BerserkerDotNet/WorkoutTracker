﻿public record MuscleExerciseSelector(Guid MuscleId, int? TargetSets = null, TimeSpan? TargetRestTime = default) : IExerciseSelector
{
    public ExerciseDescriptor Select(IEnumerable<ExerciseViewModel> exercises)
    {
        var matchedExercises = exercises.Where(e =>
        {
            var muscles = e.Muscles.Select(m => m.Id).ToArray();
            return Array.IndexOf(muscles, MuscleId) == 0;
        });

        return matchedExercises.Any() ? new ExerciseDescriptor(matchedExercises, TargetSets, TargetRestTime) : null;
    }
}

public record SpecificExerciseSelector(Guid ExerciseId, int? TargetSets = null, TimeSpan? TargetRestTime = default) : IExerciseSelector
{
    public ExerciseDescriptor Select(IEnumerable<ExerciseViewModel> exercises)
    {
        var matchedExercises = exercises.Where(e => e.Id == ExerciseId);
        return matchedExercises.Any() ? new ExerciseDescriptor(matchedExercises, TargetSets, TargetRestTime) : null;
    }
}

public record MuscleGroupExerciseSelector(string GroupName, int? TargetSets = null, TimeSpan? TargetRestTime = default) : IExerciseSelector
{
    public ExerciseDescriptor Select(IEnumerable<ExerciseViewModel> exercises)
    {
        var matchedExercises = exercises.Where(e => string.Equals(e.Muscles.First().MuscleGroup, GroupName, StringComparison.OrdinalIgnoreCase));
        return matchedExercises.Any() ? new ExerciseDescriptor(matchedExercises, TargetSets, TargetRestTime) : null;
    }

    public static implicit operator MuscleGroupExerciseSelector(string group) => new MuscleGroupExerciseSelector(group);
}