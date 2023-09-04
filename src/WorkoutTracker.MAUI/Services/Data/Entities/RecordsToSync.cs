using System;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.MAUI.Services.Data.Entities;

internal class RecordsToSync : BaseDbEntity
{
    public string TableName { get; set; }

    public Guid RecordId { get; set; }

    public OperationType OpType { get; set; }

    public RecordToSyncViewModel ToViewModel() => new(Id, TableName, RecordId, OpType);
    public static RecordsToSync FromViewModel(RecordToSyncViewModel model) => new RecordsToSync
    {
      Id = model.Id,
      TableName = model.TableName,
      RecordId = model.RecordId,
      OpType = model.OpType
    };
}
