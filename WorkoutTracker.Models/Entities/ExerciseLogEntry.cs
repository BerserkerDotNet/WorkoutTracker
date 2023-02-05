using System;
using WorkoutTracker.Models.Contracts;

namespace WorkoutTracker.Models.Entities;

[PluralName(EndpointNames.ExerciseLogEntryPluralName)]
public class ExerciseLogEntry : EntityBase
{
    public Guid ExerciseId { get; set; }

    public IExerciseSet[] Sets { get; set; }

    public DateTime Date { get; set; }
}