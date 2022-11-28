using Mapster;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Utils;

public class Mappings
{
    public static void Configure()
    {
        TypeAdapterConfig<ExerciseViewModel, Exercise>
            .NewConfig()
            .Map(dest => dest.Muscles, src => src.Muscles.Select(m => m.Id).ToArray());

        TypeAdapterConfig<Exercise, ExerciseViewModel>
            .NewConfig()
            .Ignore(dest => dest.Muscles);

        TypeAdapterConfig<LogEntryViewModel, ExerciseLogEntry>
            .NewConfig()
            .Map(dest => dest.ExerciseId, src => src.Exercise.Id);

        TypeAdapterConfig<ExerciseLogEntry, LogEntryViewModel>
            .NewConfig()
            .Ignore(dest => dest.Exercise);
    }
}
