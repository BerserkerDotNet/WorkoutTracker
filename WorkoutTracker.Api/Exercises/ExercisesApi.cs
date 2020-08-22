using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using WorkoutTracker.Models;
using System;
using Microsoft.Azure.Cosmos.Table.Queryable;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace WorkoutTracker.Api.Exercises
{
    public static class ExercisesApi
    {
        [FunctionName("Exercises")]
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

            return await new ExerciseActions(config).Run(req, log);
        }
    }

    public class ExerciseActions : HttpActionsBase<Exercise>
    {
        public ExerciseActions(IConfigurationRoot config)
            : base(config)
        {

        }

        internal override EntityWrapper<Exercise> GetEntityWrapper(Exercise entity)
        {
            return EntityWrapper<Exercise>.From(entity, "Exercises");
        }

        internal override TableQuery<EntityWrapper<Exercise>> GetQueryFromRequest(TableQuery<EntityWrapper<Exercise>> query, HttpRequest req)
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
