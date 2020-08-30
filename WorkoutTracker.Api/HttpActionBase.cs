using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WorkoutTracker.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Microsoft.Extensions.Configuration;
using WorkoutTracker.Api.Interfaces;

namespace WorkoutTracker.Api
{
    public abstract class HttpActionsBase<T>
        where T : EntityBase, new()
    {
        public HttpActionsBase(IRepository<T> repository)
        {
            this.Repository = repository;
        }

        protected ILogger Log { get; private set; }

        protected IRepository<T> Repository { get; private set; }

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

        protected virtual async ValueTask<IEnumerable<T>> Get(HttpRequest req)
        {
            if (req.Query.ContainsKey("id"))
            {
                var id = ExtractId(req);
                var item = await Repository.Get(Guid.Parse(id));
                return new[] { item };
            }

            return await Repository.Get();
        }

        protected async ValueTask<T> Post(T entity)
        {
            return await Repository.Create(entity);
        }

        protected async ValueTask<T> Patch(T entity)
        {
            return await Repository.Update(entity);
        }

        protected async ValueTask<bool> Delete(string id)
        {
            await Repository.Delete(Guid.Parse(id));
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
