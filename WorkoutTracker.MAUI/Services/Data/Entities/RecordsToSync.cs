using System;

namespace WorkoutTracker.MAUI.Services.Data.Entities;

public class RecordsToSync : BaseDbEntity
{
    public string TableName { get; set; }

    public Guid RecordId { get; set; }

    public OperationType OpType { get; set; }
}

public enum OperationType
{
    Update,
    Delete
}
