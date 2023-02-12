using SQLiteNetExtensions.Attributes;
using System;

namespace WorkoutTracker.MAUI.Services.Data.Entities;

public class MusclesToExercises
{
    [ForeignKey(typeof(ExerciseDbEntity))]
    public Guid ExerciseId { get; set; }

    [ForeignKey(typeof(MuscleDbEntity))]
    public Guid MuscleId { get; set; }
}
