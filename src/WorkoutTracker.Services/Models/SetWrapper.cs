using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Extensions;

namespace WorkoutTracker.Services.Models;

public record SetWrapper(int Number, IExerciseSet Set, LogEntryViewModel Model)
{
    public Color Color => Set.GetColor();
}

public record DataSeriesItem(string Name, int Value);


public record WorkoutStatistics(WorkoutsSummary Summary, IEnumerable<DataSeriesItem> PercentagePerMuscleGroup);

public record WorkoutsSummary(int TotalCount, int ThisWeek, int ThisMonth)
{
    public static readonly WorkoutsSummary Empty = new WorkoutsSummary(0, 0, 0);
}

public enum OperationType
{
    Update,
    Delete
}

public record RecordToSyncViewModel(Guid Id, string TableName, Guid RecordId, OperationType OpType);