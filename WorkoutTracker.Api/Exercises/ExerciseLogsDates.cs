using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using WorkoutTracker.Api.Data;
using System.Threading.Tasks;

namespace WorkoutTracker.Api.Exercises
{
    public static class ExerciseLogsDates
    {
        [FunctionName("ExerciseLogsDates")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var repo = new ATSExerciseLogRepository(config);
            var dates = await repo.GetAllDates();
            return new OkObjectResult(dates);
        }
    }
}
