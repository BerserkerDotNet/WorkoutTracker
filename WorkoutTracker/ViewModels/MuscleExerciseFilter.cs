﻿public record MuscleExerciseFilter(Guid MuscleId) : IExerciseFilter
{
    public bool Match(ExerciseViewModel exercise)
    {
        var muscles = exercise.Muscles.Select(m => m.Id).ToArray();
        var idx = Array.IndexOf(muscles, MuscleId);


        return idx == 0;
    }
}

public record SpecificExerciseFilter(Guid ExerciseId) : IExerciseFilter
{
    public bool Match(ExerciseViewModel exercise)
    {
        return exercise.Id == ExerciseId;
    }
}
