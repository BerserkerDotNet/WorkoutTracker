using Microsoft.Azure.Cosmos;
using System.Net;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Extensions;

public static class ContainerExtensions
{
    public static async Task<TEntity> UpsertEntity<TEntity>(this Container container, TEntity entity, ILogger logger)
        where TEntity : EntityBase
    {
        var response = await container.UpsertItemAsync(entity);
        if (response.StatusCode != System.Net.HttpStatusCode.Created && response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            logger.LogError("Failed to create exercise with '{StatusCode}'", response.StatusCode);
            throw new Exception($"Failed to create exercise with '{response.StatusCode}'");
        }

        logger.LogInformation("Exercise '{Id}' is created.", response.Resource.Id);

        return response.Resource;
    }

    public static async Task DeleteEntity<TEntity>(this Container container, Guid id, ILogger logger)
        where TEntity : EntityBase
    {
        logger.LogInformation("Deleting exercise '{Id}'", id);
        var response = await container.DeleteItemAsync<TEntity>(id.ToString(), new PartitionKey(id.ToString()));
        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return;
        }

        throw new Exception($"Error deleting exercise. {response.StatusCode}.");
    }
}
