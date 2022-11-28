using Microsoft.Azure.Cosmos;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Data;

public class ExerciseLogsContainer : BaseContainerDecorator
{
    public ExerciseLogsContainer(Container baseContainer)
        : base(baseContainer)
    {
    }

    public static async Task<ExerciseLogsContainer> Create(Database database)
    {
        var response = await database.CreateContainerIfNotExistsAsync(EndpointNames.ExerciseLogEntryPluralName, "/id");
        return new ExerciseLogsContainer(response.Container);
    }
}
