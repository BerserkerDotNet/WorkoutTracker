using Microsoft.Azure.Cosmos;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Data;

public class WorkoutProgramsContainer : BaseContainerDecorator
{
    public WorkoutProgramsContainer(Container baseContainer)
        : base(baseContainer)
    {
    }

    public static async Task<WorkoutProgramsContainer> Create(Database database)
    {
        var response = await database.CreateContainerIfNotExistsAsync(EndpointNames.WorkoutProgramPluralName, "/id");
        return new WorkoutProgramsContainer(response.Container);
    }
}
