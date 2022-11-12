using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using System;
using System.Linq;
using Azure.Identity;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Functions
{
    public static class CosmosUtils 
    {
        public static async Task<Container> GetContainer<T>()
            where T : EntityBase
        {
            var endpoint = Environment.GetEnvironmentVariable("DbEndpoint");
            var client = new CosmosClient(endpoint, new DefaultAzureCredential());
            Database database = await client.CreateDatabaseIfNotExistsAsync("WorkoutTrackerData");
            return await database.CreateContainerIfNotExistsAsync(GetNameFor<T>(), "/id");
        }

        private static string GetNameFor<T>() => typeof(T).GetCustomAttributes(typeof(PluralNameAttribute), false).Cast<PluralNameAttribute>().Single().PluralName;
    }
}
