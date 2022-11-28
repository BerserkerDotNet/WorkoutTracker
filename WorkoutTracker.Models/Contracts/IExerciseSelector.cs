using System;
using System.Collections.Generic;

namespace WorkoutTracker.Models.Contracts;

public record ExerciseDescriptor(IEnumerable<ExerciseViewModel> MatchedExercises, int? TargetSets = null, TimeSpan? TargetRestTime = null);

public interface IExerciseSelector
{
    ExerciseDescriptor Select(IEnumerable<ExerciseViewModel> exercises);
}
