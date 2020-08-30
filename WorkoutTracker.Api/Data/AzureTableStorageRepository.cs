using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Cosmos.Table.Queryable;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Api.Interfaces;
using WorkoutTracker.Models;

namespace WorkoutTracker.Api.Data
{
    public class AzureTableStorageRepository<T> : IRepository<T>
        where T: EntityBase, new()
    {
        protected static Guid _marker = new Guid("466528cb-2a56-4921-85a5-886b640723f6");

        public AzureTableStorageRepository(IConfigurationRoot config)
        {
            var prefix = config.GetValue<string>("TablePrefix");
            var storageConString = config.GetConnectionString("StorageConnection");
            var storageAccount = CloudStorageAccount.Parse(storageConString);
            var client = storageAccount.CreateCloudTableClient();
            Table = client.GetTableReference($"{prefix}{typeof(T).Name}");
            Table.CreateIfNotExists();
        }

        protected CloudTable Table { get; }

        public ValueTask<IEnumerable<T>> Get()
        {
            var query = Table.CreateQuery<EntityWrapper<T>>()
                .Where(w => w.Entity.Id != _marker)
                .AsTableQuery();

            var result = Table.ExecuteQuery(query).Select(s => s.Entity);
            return new ValueTask<IEnumerable<T>>(Task.FromResult(result));
        }

        public ValueTask<T> Get(Guid id)
        {
            var query = Table.CreateQuery<EntityWrapper<T>>()
                .Where(w => w.Entity.Id == id)
                .AsTableQuery();

            var wrapper = Table.ExecuteQuery(query).FirstOrDefault();
            var result = wrapper == null ? null : wrapper.Entity;
            return new ValueTask<T>(Task.FromResult(result));
        }

        public async ValueTask<T> Create(T entity)
        {
            var wrapper = GetEntityWrapper(entity);
            var operation = TableOperation.InsertOrReplace(wrapper);
            await Table.ExecuteAsync(operation);

            var markerWrapper = EntityWrapper<T>.From(new T() { Id = _marker }, wrapper.PartitionKey);
            var markerOperation = TableOperation.InsertOrReplace(markerWrapper);
            await Table.ExecuteAsync(markerOperation);

            return entity;
        }

        public async ValueTask Delete(Guid id)
        {
            var entities = Table.ExecuteQuery(Table.CreateQuery<EntityWrapper<T>>().Where(q => q.Entity.Id == id).AsTableQuery());
            if (!entities.Any())
            {
                return;
            }

            var deleteOperation = TableOperation.Delete(entities.First());
            await Table.ExecuteAsync(deleteOperation);
        }

        public async ValueTask<T> Update(T entity)
        {
            var wrapper = GetEntityWrapper(entity);
            var operation = TableOperation.Merge(wrapper);
            await Table.ExecuteAsync(operation);

            var readOperation = TableOperation.Retrieve(wrapper.PartitionKey, wrapper.RowKey);
            var operationResult = await Table.ExecuteAsync(readOperation);

            return (T)operationResult.Result;
        }

        private EntityWrapper<T> GetEntityWrapper(T entity)
        {
            string partitionKey;
            switch (entity)
            {
                case Exercise _:
                    partitionKey = "Exercises";
                    break;
                case ExerciseLogEntry log:
                    partitionKey = log.Date.ToString("dd-MM-yyyy");
                    break;
                default:
                    throw new NotSupportedException($"Type {typeof(T)} is not supported.");
            }

            return EntityWrapper<T>.From(entity, partitionKey);
        }
    }

    public class ATSExerciseLogRepository : AzureTableStorageRepository<ExerciseLogEntry>, IExerciseLogRepository
    {
        public ATSExerciseLogRepository(IConfigurationRoot config)
            : base(config)
        {
        }

        public ValueTask<IEnumerable<string>> GetAllDates()
        {
            var query = Table.CreateQuery<EntityWrapper<ExerciseLogEntry>>().Where(w => w.Entity.Id == _marker).AsTableQuery();
            var result = Table.ExecuteQuery(query).Select(s => s.PartitionKey);
            return new ValueTask<IEnumerable<string>>(Task.FromResult(result));
        }

        public ValueTask<IEnumerable<ExerciseLogEntry>> GetByDate(string date)
        {
            var query = Table.CreateQuery<EntityWrapper<ExerciseLogEntry>>().Where(w => w.PartitionKey == date && w.Entity.Id != _marker).AsTableQuery();
            var result = Table.ExecuteQuery(query).Select(s => s.Entity);

            return new ValueTask<IEnumerable<ExerciseLogEntry>>(Task.FromResult(result));
        }
    }
}
