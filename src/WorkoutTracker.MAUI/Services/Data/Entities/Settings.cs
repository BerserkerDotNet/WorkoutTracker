using System;

namespace WorkoutTracker.MAUI.Services.Data.Entities;

public class Settings : BaseDbEntity
{
    public DateTime LastSyncDate { get; set; }
}