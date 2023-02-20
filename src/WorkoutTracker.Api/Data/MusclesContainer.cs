using Microsoft.Azure.Cosmos;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Data
{
    public class MusclesContainer : BaseContainerDecorator
    {
        public MusclesContainer(Container baseContainer)
            : base(baseContainer)
        {
        }

        public static async Task<MusclesContainer> Create(Database database)
        {
            var response = await database.CreateContainerIfNotExistsAsync(EndpointNames.MusclePluralName, "/id");
            return new MusclesContainer(response.Container);
        }
    }
}
