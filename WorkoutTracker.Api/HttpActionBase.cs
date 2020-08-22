using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;
using WorkoutTracker.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Extensions.Configuration;

namespace WorkoutTracker.Api
{
    public abstract class HttpActionsBase<T>
        where T : EntityBase, new()
    {
        private CloudTable _table;

        public HttpActionsBase(IConfigurationRoot config)
        {
            var prefix = config.GetValue<string>("TablePrefix");
            var storageConString = config.GetConnectionString("StorageConnection");
            var storageAccount = CloudStorageAccount.Parse(storageConString);
            var client = storageAccount.CreateCloudTableClient();
            _table = client.GetTableReference($"{prefix}{typeof(T).Name}");
            _table.CreateIfNotExists();
        }

        protected ILogger Log { get; private set; }

        public async Task<IActionResult> Run(HttpRequest req, ILogger log)
        {
            object result;
            try
            {
                
                Log = log;
                switch (req.Method.ToUpper())
                {
                    case "GET":
                        result = await Get(req);
                        break;
                    case "POST":
                        var modelToCreate = await ExtractModel(req);
                        result = await Post(modelToCreate);
                        break;
                    case "PATCH":
                        var modelToUpdate = await ExtractModel(req);
                        result = await Patch(modelToUpdate);
                        break;
                    case "DELETE":
                        result = await Delete(ExtractId(req));
                        break;
                    default:
                        throw new NotSupportedException($"'{req.Method}' is not supported.");
                }
            }
            catch (BadRequestException ex)
            {
                log.LogWarning(ex, "Request validation exception");
                return new BadRequestObjectResult(ex.Message);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing request");
                return new InternalServerErrorResult();
            }

            return new OkObjectResult(result);
        }

        internal abstract EntityWrapper<T> GetEntityWrapper(T entity);

        internal abstract TableQuery<EntityWrapper<T>> GetQueryFromRequest(TableQuery<EntityWrapper<T>> query, HttpRequest req);

        protected ValueTask<IEnumerable<T>> Get(HttpRequest req)
        {
            var query = GetQueryFromRequest(_table.CreateQuery<EntityWrapper<T>>(), req);
            var result = _table.ExecuteQuery(query);

            return new ValueTask<IEnumerable<T>>(Task.FromResult(result.Select(w => w.Entity)));
        }

        protected async ValueTask<T> Post(T entity)
        {
            var wrapper = GetEntityWrapper(entity);
            var operation = TableOperation.Insert(wrapper);
            await _table.ExecuteAsync(operation);

            return entity;
        }

        protected async ValueTask<T> Patch(T entity)
        {
            var wrapper = GetEntityWrapper(entity);
            var operation = TableOperation.Merge(wrapper);
            await _table.ExecuteAsync(operation);

            var readOperation = TableOperation.Retrieve(wrapper.PartitionKey, wrapper.RowKey);
            var operationResult = await _table.ExecuteAsync(readOperation);

            return (T)operationResult.Result;
        }

        protected async ValueTask<bool> Delete(string id)
        {
            var entities = _table.ExecuteQuery(_table.CreateQuery<EntityWrapper<T>>().Where(q => q.RowKey == id).AsTableQuery());
            if (!entities.Any())
            {
                return false;
            }

            var deleteOperation  = TableOperation.Delete(entities.First());
            await _table.ExecuteAsync(deleteOperation);

            return true;
        }

        protected string ExtractId(HttpRequest req)
        {
            var ids = req.Query["id"];

            if (!ids.Any())
            {
                throw new BadRequestException("Id is missing.");
            }

            return ids.First();
        }

        private async ValueTask<T> ExtractModel(HttpRequest req)
        {
            try
            {
                var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<T>(requestBody);
                return model;
            }
            catch (Exception ex)
            {
                throw new BadRequestException("Error getting the model", ex);
            }
        }
    }
}
