using System;

namespace WorkoutTracker.Models.Presentation;

public class MuscleViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string MuscleGroup { get; set; }

    public string ImagePath { get; set; }
}
