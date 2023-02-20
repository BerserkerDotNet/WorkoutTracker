using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.MAUI.Services.Data.Entities;

public class LogsDbEntity : BaseDbEntity
{
    [ForeignKey(typeof(ExerciseDbEntity))]
    public Guid ExerciseId { get; set; }

    [OneToOne(nameof(ExerciseId), CascadeOperations = CascadeOperation.CascadeRead)]
    public ExerciseDbEntity Exercise { get; set; }

    public int Order { get; set; }

    public DateTime Date { get; set; }

    [TextBlob(nameof(SetsBlobbed))]
    public IEnumerable<IExerciseSet> Sets { get; set; }

    public string SetsBlobbed { get; set; }

    public LogEntryViewModel ToViewModel()
    {
        return new LogEntryViewModel
        {
            Id = Id,
            Date = Date,
            Exercise = Exercise.ToViewModel(),
            Order = Order,
            Sets = new ObservableCollection<IExerciseSet>(Sets)
        };
    }

    public static LogsDbEntity FromViewModel(LogEntryViewModel viewModel)
    {
        return new LogsDbEntity
        {
            Id = viewModel.Id,
            Date = viewModel.Date,
            Order = viewModel.Order,
            Exercise = ExerciseDbEntity.FromViewModel(viewModel.Exercise),
            Sets = viewModel.Sets
        };
    }
}
