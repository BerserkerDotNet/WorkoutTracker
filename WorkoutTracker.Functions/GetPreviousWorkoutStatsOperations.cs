using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WorkoutTracker.Models;
using System;

namespace WorkoutTracker.Functions
{
    public static class GetPreviousWorkoutStatsOperations
    {
        [Authorize]
        [FunctionName(EndpointNames.GetPreviousWorkoutStatsByExercise)]
        public static Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest request,
            ILogger log)
        {
            switch (request.Method)
            {
                case "GET":
                    return GetPreviousWorkoutStatsByExercise(request, log);
                default:
                    return Task.FromResult<IActionResult>(new BadRequestObjectResult($"Method {request.Method} is not supported."));
            }
        }

        private static async Task<IActionResult> GetPreviousWorkoutStatsByExercise(HttpRequest request, ILogger log)
        {
            var isValidIdParametert = Guid.TryParse(request.Query["exerciseId"], out var id);
            if (!isValidIdParametert) 
            {
                return new BadRequestObjectResult("Exercise ID parameter is required and must be a valid Guid.");
            }

            var container = await CosmosUtils.GetContainer<ExerciseLogEntry>();
            var today = DateTime.Today.ToUniversalTime();
            var lastWorkoutLogEntries = container.GetItemLinqQueryable<ExerciseLogEntry>(allowSynchronousQueryExecution: true)
                .Where(e => e.Date < today && e.ExerciseId == id)
                .OrderByDescending(e => e.Date)
                .Take(1);

            return new OkObjectResult(lastWorkoutLogEntries.AsEnumerable().FirstOrDefault());
        }
    }
}
