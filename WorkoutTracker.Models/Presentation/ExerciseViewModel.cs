using System;
using System.Collections.Generic;

public class ExerciseViewModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Steps { get; set; }

    public string TutorialUrl { get; set; }

    public string ImagePath { get; set; }

    public IEnumerable<MuscleViewModel> Muscles { get; set; }

    public IEnumerable<string> Tags { get; set; }
}
