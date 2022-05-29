using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WorkoutTracker.Models;
using Microsoft.Azure.Cosmos;
using System;

namespace WorkoutTracker.Functions;
public static class ExerciseLogOperations
{
    [Authorize]
    [FunctionName(EndpointNames.ExerciseLogEntryPluralName)]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "delete", "patch", Route = null)] HttpRequest request,
        ILogger log)
    {
        try
        {
            log.LogInformation("Processing '{Method}' request to '{Entity}'", request.Method, EndpointNames.ExerciseLogEntryPluralName);
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
            log.LogError(ex, "Error processing '{Method}' on '{Entity}'", request.Method, EndpointNames.ExerciseLogEntryPluralName);
            throw;
        }
    }

    private static async Task<IActionResult> Get(HttpRequest request, ILogger log)
    {
        var id = request.Query["id"];
        var date = !string.IsNullOrEmpty(request.Query["date"]) ? DateTime.Parse(request.Query["date"]) : DateTime.Today.ToUniversalTime();
        var nextDay = date.AddDays(1);
        var container = await CosmosUtils.GetContainer<ExerciseLogEntry>();

        if (string.IsNullOrEmpty(id))
        {
            log.LogInformation("Fetching logs for '{Date}'", date);
            var items = container.GetItemLinqQueryable<ExerciseLogEntry>(allowSynchronousQueryExecution: true)
                .Where(e => e.Date >= date && e.Date < nextDay)
                .OrderByDescending(e => e.Date);

            return new OkObjectResult(items);
        }
        else
        {
            log.LogInformation("Fetching exericse log '{Id}'", id);
            var item = await container.ReadItemAsync<ExerciseLogEntry>(id, new PartitionKey(id));
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

        log.LogInformation("Deleting exercise log '{Id}'", id);
        var container = await CosmosUtils.GetContainer<ExerciseLogEntry>();
        var response = await container.DeleteItemAsync<ExerciseLogEntry>(id, new PartitionKey(id));
        return new StatusCodeResult((int)response.StatusCode);
    }

    private static async Task<IActionResult> Create(HttpRequest request, ILogger log)
    {
        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        var entry = JsonConvert.DeserializeObject<ExerciseLogEntry>(requestBody);
        var container = await CosmosUtils.GetContainer<ExerciseLogEntry>();
        var response = await container.UpsertItemAsync(entry);

        if (response.StatusCode != System.Net.HttpStatusCode.Created)
        {
            log.LogError("Failed to create exercise log with '{StatusCode}'", response.StatusCode);
            return new StatusCodeResult((int)response.StatusCode);
        }

        log.LogInformation("Exercise log '{Id}' is created.", response.Resource.Id);
        return new OkObjectResult(response.Resource);
    }
}
