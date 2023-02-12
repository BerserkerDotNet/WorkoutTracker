using SQLite;
using System;

namespace WorkoutTracker.MAUI.Services.Data.Entities;

public abstract class BaseDbEntity
{
    [PrimaryKey]
    public Guid Id { get; set; }
}