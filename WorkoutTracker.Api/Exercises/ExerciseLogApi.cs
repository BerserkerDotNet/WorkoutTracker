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
            : base(config)
        {

        }

        internal override EntityWrapper<ExerciseLogEntry> GetEntityWrapper(ExerciseLogEntry entity)
        {
            var partitionKey = entity.Date.ToString("dd-MM-yyyy");
            return EntityWrapper<ExerciseLogEntry>.From(entity, partitionKey);
        }

        internal override TableQuery<EntityWrapper<ExerciseLogEntry>> GetQueryFromRequest(TableQuery<EntityWrapper<ExerciseLogEntry>> query, HttpRequest req)
        {
            if (req.Query.ContainsKey("id"))
            {
                var id = ExtractId(req);
                return query.Where(w => w.RowKey == id).AsTableQuery();
            }

            if (req.Query.ContainsKey("query"))
            {
            }

            return query;
        }
    }
}
