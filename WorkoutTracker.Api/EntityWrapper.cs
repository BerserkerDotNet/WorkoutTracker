using System;
using Microsoft.Azure.Cosmos.Table;
using WorkoutTracker.Models;
using System.Collections.Generic;

namespace WorkoutTracker.Api
{
    internal class EntityWrapper<T> : ITableEntity
        where T : EntityBase, new()
    {
        public EntityWrapper()
        {
            Entity = new T();
        }

        public EntityWrapper(T entity)
        {
            Entity = entity;
        }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public string ETag { get; set; } = "*";

        public T Entity { get; private set; }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            Entity = TableEntity.ConvertBack<T>(properties, operationContext);
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            return TableEntity.Flatten(Entity, operationContext);
        }

        public static EntityWrapper<T> From(T entity, string partitionKey)
        {
            return new EntityWrapper<T>(entity)
            {
                PartitionKey = partitionKey,
                RowKey = entity.Id.ToString(),
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
