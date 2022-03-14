using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WorkoutTracker.Models;
using Microsoft.Azure.Cosmos;
using System.IO;
using Newtonsoft.Json;

namespace WorkoutTracker.Functions
{
    public static class MuscleOperations
    {
        [Authorize]
        [FunctionName(EndpointNames.MusclePluralName)]
        public static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "delete", "patch", Route = null)] HttpRequest request,
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
            var container = await CosmosUtils.GetContainer<Muscle>();

            if (string.IsNullOrEmpty(id))
            {
                var items = container.GetItemLinqQueryable<Muscle>(allowSynchronousQueryExecution: true);
                return new OkObjectResult(items);
            }
            else
            {
                var item = await container.ReadItemAsync<Muscle>(id, new PartitionKey(id));
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

            var container = await CosmosUtils.GetContainer<Muscle>();
            var response = await container.DeleteItemAsync<Muscle>(id, new PartitionKey(id));
            return new StatusCodeResult((int)response.StatusCode);
        }

        private static async Task<IActionResult> Create(HttpRequest request, ILogger log)
        {
            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var entry = JsonConvert.DeserializeObject<Muscle>(requestBody);
            var container = await CosmosUtils.GetContainer<Muscle>();
            var response = await container.UpsertItemAsync(entry);

            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                return new StatusCodeResult((int)response.StatusCode);
            }

            return new OkObjectResult(response.Resource);
        }
    }
}
