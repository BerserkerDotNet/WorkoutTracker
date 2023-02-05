using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using WorkoutTracker.Models.Contracts;

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

[PluralName(EndpointNames.WorkoutProgramPluralName)]
public class WorkoutProgram : EntityBase
{
    public required string Name { get; set; }

    public required Schedule Schedule { get; set; }
}

public class Schedule
{
    public required WorkoutDefinition Monday { get; set; }

    public required WorkoutDefinition Tuesday { get; set; }

    public required WorkoutDefinition Wednesday { get; set; }

    public required WorkoutDefinition Thursday { get; set; }

    public required WorkoutDefinition Friday { get; set; }

    public required WorkoutDefinition Saturday { get; set; }

    public required WorkoutDefinition Sunday { get; set; }
}

public class WorkoutDefinition
{
    public required string Name { get; set; }

    public IList<ExerciseDefinition> Exercises { get; set; }

    public static WorkoutDefinition Rest => new WorkoutDefinition() { Name = "Rest", Exercises = Enumerable.Empty<ExerciseDefinition>().ToList() };
}

public class ExerciseDefinition
{
    public IExerciseSelector ExerciseSelector { get; set; }

    public required IProgressiveOverloadFactor OverloadFactor { get; set; }
}

[JsonDerivedType(typeof(OneRepMaxProgressiveOverloadFactor), nameof(OneRepMaxProgressiveOverloadFactor))]
[JsonDerivedType(typeof(SteadyStateProgressiveOverloadFactor), nameof(SteadyStateProgressiveOverloadFactor))]
public interface IProgressiveOverloadFactor
{
    string GetDisplayText();
}

public record class OneRepMaxProgressiveOverloadFactor(int Percentage, int NumberOfSets) : IProgressiveOverloadFactor
{
    public string GetDisplayText() => $"{Percentage}% of 1RM";
}

public record class SteadyStateProgressiveOverloadFactor(int Weight, int NumberOfSets, int NumberOfReps) : IProgressiveOverloadFactor
{
    public string GetDisplayText() => $"Steady state";
}

public record class PowerLadderOverloadFactor(int StepIncrement, int Overload, int WarmupSets, int WorkingSets, int TargetReps) : IProgressiveOverloadFactor
{
    public string GetDisplayText() => $"Ladder of {StepIncrement}LB with {Overload}LB bump";
}

public enum ProgressiveOverloadType
{
    PowerLadder,
    OneRepMaxPercentage
}

public enum ExerciseSelectorType
{
    SpecificExercise,
    ExerciseGroup,
    SpecificMuscle,
    MuscleGroup
}