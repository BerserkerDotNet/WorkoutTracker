using Microsoft.Azure.Cosmos;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Data
{
    public class UsersContainer : BaseContainerDecorator
    {
        public UsersContainer(Container baseContainer)
            : base(baseContainer)
        {
        }

        public static async Task<UsersContainer> Create(Database database)
        {
            var response = await database.CreateContainerIfNotExistsAsync(EndpointNames.UserPluralName, "/id");
            return new UsersContainer(response.Container);
        }
    }
}