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

namespace WorkoutTracker.Functions
{
    public static class ExerciseLogOperations
    {
        [FunctionName(EntityPluralNames.ExerciseLogEntryPluralName)]
        public static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", "delete", "patch", Route = null)] HttpRequest request,
            ILogger log)
        {
            switch (request.Method)
            {
                case "GET":
                    return Get(request, log);
                case "POST":
                    return Create(request, log);
                case "DELETE":
                    return Delete(request, log);
                default:
                    return Task.FromResult<IActionResult>(new BadRequestObjectResult($"Method {request.Method} is not supported."));
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
                var items = container.GetItemLinqQueryable<ExerciseLogEntry>(allowSynchronousQueryExecution: true)
                    .Where(e => e.Date >= date && e.Date < nextDay)
                    .OrderByDescending(e => e.Date);

                return new OkObjectResult(items);
            }
            else
            {
                var item = await container.ReadItemAsync<ExerciseLogEntry>(id, new PartitionKey(id));
                return new OkObjectResult(item.Resource);
            }
        }

        private static async Task<IActionResult> Delete(HttpRequest request, ILogger log)
        {
            var id = request.Query["id"];
            if (string.IsNullOrEmpty(id))
            {
                return new BadRequestObjectResult("Id of an item is required.");
            }

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
                return new StatusCodeResult((int)response.StatusCode);
            }

            return new OkObjectResult(response.Resource);
        }
    }
}
