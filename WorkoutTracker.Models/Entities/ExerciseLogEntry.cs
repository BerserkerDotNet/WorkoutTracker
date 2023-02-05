using System;
using System.Linq;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.Models.Entities;

[PluralName(EndpointNames.ExerciseLogEntryPluralName)]
public class ExerciseLogEntry : EntityBase
{
    public Guid ExerciseId { get; set; }

    public IExerciseSet[] Sets { get; set; }

    public DateTime Date { get; set; }

    public static ExerciseLogEntry FromViewModel(LogEntryViewModel vm)
    {
        return new ExerciseLogEntry
        {
            Id = vm.Id,
            Date = vm.Date,
            ExerciseId = vm.Exercise.Id,
            Sets = vm.Sets.ToArray()
        };
    }

    public LogEntryViewModel ToViewModel(ExerciseViewModel exercise)
    {
        return new LogEntryViewModel
        {
            Id = Id,
            Date = Date,
            Sets = Sets,
            Exercise = exercise
        };
    }
}