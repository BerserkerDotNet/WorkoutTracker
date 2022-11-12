using System;
using System.Collections.Generic;

namespace WorkoutTracker.Models.Entities;

[PluralName(EndpointNames.ExercisePluralName)]
public class Exercise : EntityBase
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Steps { get; set; }

    public string TutorialUrl { get; set; }

    public string ImagePath { get; set; }

    public Guid[] Muscles { get; set; }

    public string[] Tags { get; set; }
}

[PluralName("WorkoutSchedule")]
public class WorkoutSchedule : EntityBase
{
}

[PluralName("WorkoutProfiles")]
public class WorkoutProfile : EntityBase
{
    public string Name { get; set; }

    public IEnumerable<WorkoutExerciseDecriptor> ExerciseDecriptors { get; set; }
}

public class WorkoutExerciseDecriptor
{
    public string ExerciseSelectorType { get; set; }

    public Dictionary<string, object> ExerciseSelectorParameters { get; set; }

    public string SetsProviderType { get; set; }

    public TimeSpan DefaultRestTime { get; set; }

    public TimeSpan DefaultSetsCount { get; set; }
}