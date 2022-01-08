using System.Threading.Tasks;
using WorkoutTracker.Models;
using Microsoft.Azure.Cosmos;
using System;

namespace WorkoutTracker.Functions
{
    public static class CosmosUtils 
    {
        public static async Task<Container> GetContainer<T>()
            where T : EntityBase
        {
            var endpoint = Environment.GetEnvironmentVariable("DbEndpoint");
            var key = Environment.GetEnvironmentVariable("DbKey");
            CosmosClient client = new CosmosClient(endpoint, key); // TODO: dispose?
            Database database = await client.CreateDatabaseIfNotExistsAsync("WorkoutTrackerData");
            return await database.CreateContainerIfNotExistsAsync(typeof(T).Name, "/id");
        }
    }
}
