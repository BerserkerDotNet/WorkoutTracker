using Microsoft.Azure.Cosmos;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Data;

public class ExercisesContainer : BaseContainerDecorator
{
    public ExercisesContainer(Container baseContainer)
        : base(baseContainer)
    {
    }

    public static async Task<ExercisesContainer> Create(Database database)
    {
        var response = await database.CreateContainerIfNotExistsAsync(EndpointNames.ExercisePluralName, "/id");
        return new ExercisesContainer(response.Container);
    }
}