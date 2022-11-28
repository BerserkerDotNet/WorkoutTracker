using System;

namespace WorkoutTracker.Models.Entities;

[PluralName(EndpointNames.ExerciseLogEntryPluralName)]
public class ExerciseLogEntry : EntityBase
{
    public Guid ExerciseId { get; set; }

    public Set[] Sets { get; set; }

    public DateTime Date { get; set; }
}