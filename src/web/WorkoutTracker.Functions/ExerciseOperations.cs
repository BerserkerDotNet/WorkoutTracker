using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;
using System.IO;
using Newtonsoft.Json;
using System;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Functions;

public static class ExerciseOperations
{
    [Authorize]
    [FunctionName(EndpointNames.ExercisePluralName)]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "delete", "patch", Route = null)] HttpRequest request,
        ILogger log)
    {
        try
        {
            log.LogInformation("Processing '{Method}' request to '{Entity}'", request.Method, EndpointNames.ExercisePluralName);
            switch (request.Method)
            {
                case "GET":
                    return await Get(request, log);
                case "POST":
                    return await Create(request, log);
                case "DELETE":
                    return await Delete(request, log);
                default:
                    return new BadRequestObjectResult($"Method {request.Method} is not supported.");
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Error processing '{Method}' on '{Entity}'", request.Method, EndpointNames.ExercisePluralName);
            throw;
        }
    }

    private static async Task<IActionResult> Get(HttpRequest request, ILogger log)
    {
        var id = request.Query["id"];
        var container = await CosmosUtils.GetContainer<Exercise>();

        if (string.IsNullOrEmpty(id))
        {
            log.LogInformation("Fetching all exercises");
            var items = container.GetItemLinqQueryable<Exercise>(allowSynchronousQueryExecution: true);
            return new OkObjectResult(items);
        }
        else
        {
            log.LogInformation("Fetching exercise '{Id}'", id);
            var item = await container.ReadItemAsync<Exercise>(id, new PartitionKey(id));
            return new OkObjectResult(item.Resource);
        }
    }

    private static async Task<IActionResult> Delete(HttpRequest request, ILogger log)
    {
        var id = request.Query["id"];
        if (string.IsNullOrEmpty(id))
        {
            log.LogWarning("Failed deleting exercise. Id of an item is required.");
            return new BadRequestObjectResult("Id of an item is required.");
        }

        log.LogInformation("Deleting exercise '{Id}'", id);
        var container = await CosmosUtils.GetContainer<Exercise>();
        var response = await container.DeleteItemAsync<Exercise>(id, new PartitionKey(id));
        return new StatusCodeResult((int)response.StatusCode);
    }

    private static async Task<IActionResult> Create(HttpRequest request, ILogger log)
    {
        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        var entry = JsonConvert.DeserializeObject<Exercise>(requestBody);
        var container = await CosmosUtils.GetContainer<Exercise>();
        var response = await container.UpsertItemAsync(entry);

        if (response.StatusCode != System.Net.HttpStatusCode.Created)
        {
            log.LogError("Failed to create exercise with '{StatusCode}'", response.StatusCode);
            return new StatusCodeResult((int)response.StatusCode);
        }

        log.LogInformation("Exercise '{Id}' is created.", response.Resource.Id);
        return new OkObjectResult(response.Resource);
    }
}
