using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.Models.Mappings;

public static class Mappings
{
    public const string MusclesLookup = "muscles";

    public static void Configure()
    {
        TypeAdapterConfig<Exercise, ExerciseViewModel>
            .NewConfig()
            .Map(e => e.Muscles, e => MapMuscles(e.Muscles));
    }

    public static ExerciseViewModel AdaptToViewModel(this Exercise source, IEnumerable<MuscleViewModel> muscles)
    {
        return source.BuildAdapter()
            .AddParameters(MusclesLookup, muscles)
            .AdaptToType<ExerciseViewModel>();
    }

    public static IEnumerable<ExerciseViewModel> AdaptToViewModel(this IEnumerable<Exercise> source, IEnumerable<MuscleViewModel> muscles)
    {
        return source.BuildAdapter()
            .AddParameters(MusclesLookup, muscles)
            .AdaptToType<IEnumerable<ExerciseViewModel>>();
    }

    private static IEnumerable<MuscleViewModel> MapMuscles(IEnumerable<Guid> ids)
    {
        var allMuscles = (IEnumerable<MuscleViewModel>)MapContext.Current.Parameters[MusclesLookup];
        return allMuscles.Where(m => ids.Contains(m.Id)).ToArray();
    }
}
