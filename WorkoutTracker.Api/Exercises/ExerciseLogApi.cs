using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WorkoutTracker.Models;
using Microsoft.Azure.Cosmos.Table.Queryable;
using System.Linq;
using Microsoft.Extensions.Configuration;
using WorkoutTracker.Api.Data;
using System.Collections.Generic;
using WorkoutTracker.Api.Interfaces;

namespace WorkoutTracker.Api.Exercises
{
    public static class ExerciseLogApi
    {
        [FunctionName("ExerciseLogs")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", "patch", "delete", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return await new ExerciseLogActions(config).Run(req, log);
        }
    }

    public class ExerciseLogActions : HttpActionsBase<ExerciseLogEntry>
    {
        public ExerciseLogActions(IConfigurationRoot config)
            : base(new ATSExerciseLogRepository(config))
        {
        }

        protected override async ValueTask<IEnumerable<ExerciseLogEntry>> Get(HttpRequest req)
        {
            var repo = Repository as IExerciseLogRepository;
            if (repo is object && req.Query.ContainsKey("date"))
            {
                var date = ExtractDate(req);
                return await repo.GetByDate(date);
            }

            return await base.Get(req);
        }

        private string ExtractDate(HttpRequest req)
        {
            var dates = req.Query["date"];

            if (!dates.Any())
            {
                throw new BadRequestException("Date is missing.");
            }

            return dates.First();
        }
    }
}
