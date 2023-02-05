using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.Models.Contracts;

public record ExerciseDescriptor(IEnumerable<ExerciseViewModel> MatchedExercises, int? TargetSets = null, TimeSpan? TargetRestTime = null);

[JsonDerivedType(typeof(SpecificExerciseSelector), nameof(SpecificExerciseSelector))]
[JsonDerivedType(typeof(MuscleGroupExerciseSelector), nameof(MuscleGroupExerciseSelector))]
[JsonDerivedType(typeof(MuscleExerciseSelector), nameof(MuscleExerciseSelector))]
public interface IExerciseSelector
{
    ExerciseDescriptor Select(IEnumerable<ExerciseViewModel> exercises);
}