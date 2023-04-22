using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Presentation;

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

    public ExerciseViewModel ToViewModel(IEnumerable<MuscleViewModel> musclesList)
    {
        return new ExerciseViewModel
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Steps = Steps,
            TutorialUrl = TutorialUrl,
            ImagePath = ImagePath,
            Tags = Tags,
            Muscles = musclesList.Where(m => Muscles.Contains(m.Id)).ToArray()
        };
    }

    public static Exercise FromViewModel(ExerciseViewModel vm)
    {
        return new Exercise
        {
            Id = vm.Id,
            Name = vm.Name,
            Description = vm.Description,
            Steps = vm.Steps,
            TutorialUrl = vm.TutorialUrl,
            ImagePath = vm.ImagePath,
            Tags = vm.Tags.ToArray(),
            Muscles = vm.Muscles.Select(m => m.Id).ToArray()
        };
    }

}

public record AssignedWorkoutDefinition(DayOfWeek Day, string DayOfWeekName, WorkoutDefinition Definition);

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
[JsonDerivedType(typeof(PowerLadderOverloadFactor), nameof(PowerLadderOverloadFactor))]
[JsonDerivedType(typeof(RepetitionsLadderOverloadFactor), nameof(RepetitionsLadderOverloadFactor))]
public interface IProgressiveOverloadFactor
{
    string DisplayText { get; }
}

public class OneRepMaxProgressiveOverloadFactor : IProgressiveOverloadFactor
{
    public OneRepMaxProgressiveOverloadFactor(int percentage, int numberOfSets)
    {
        this.Percentage = percentage;
        this.NumberOfSets = numberOfSets;
    }

    public string DisplayText => $"{Percentage}% of 1RM";
    public int Percentage { get; set; }
    public int NumberOfSets { get; set; }

    public void Deconstruct(out int percentage, out int numberOfSets)
    {
        percentage = this.Percentage;
        numberOfSets = this.NumberOfSets;
    }
}

public class SteadyStateProgressiveOverloadFactor : IProgressiveOverloadFactor
{
    public SteadyStateProgressiveOverloadFactor(int weight, int numberOfSets, int numberOfReps)
    {
        this.Weight = weight;
        this.NumberOfSets = numberOfSets;
        this.NumberOfReps = numberOfReps;
    }

    public string DisplayText => $"Steady state";
    public int Weight { get; set; }
    public int NumberOfSets { get; set; }
    public int NumberOfReps { get; set; }

    public void Deconstruct(out int weight, out int numberOfSets, out int numberOfReps)
    {
        weight = this.Weight;
        numberOfSets = this.NumberOfSets;
        numberOfReps = this.NumberOfReps;
    }
}

public class PowerLadderOverloadFactor : IProgressiveOverloadFactor
{
    public PowerLadderOverloadFactor(int stepIncrement, bool includeWarmup, int workingSets = 3, int workingReps = 5)
    {
        this.StepIncrement = stepIncrement;
        this.IncludeWarmup = includeWarmup;
        this.WorkingSets = workingSets;
        this.WorkingReps = workingReps;
    }

    public string DisplayText => $"Ladder of {StepIncrement}LB.";
    public int StepIncrement { get; set; }
    public bool IncludeWarmup { get; set; }
    public int WorkingSets { get; set; }
    public int WorkingReps { get; set; }

    public void Deconstruct(out int stepIncrement, out bool includeWarmup, out int workingSets, out int workingReps)
    {
        stepIncrement = this.StepIncrement;
        includeWarmup = this.IncludeWarmup;
        workingSets = this.WorkingSets;
        workingReps = this.WorkingReps;
    }
}

public class RepetitionsLadderOverloadFactor : IProgressiveOverloadFactor
{
    public int StepIncrement { get; set; }
    public bool IncludeWarmup { get; set; }
    public int WorkingSets { get; set; }
    public int StartingReps { get; set; }

    public RepetitionsLadderOverloadFactor(int stepIncrement, bool includeWarmup, int workingSets = 3, int startingReps = 5)
    {
        StepIncrement = stepIncrement;
        IncludeWarmup = includeWarmup;
        WorkingSets = workingSets;
        StartingReps = startingReps;
    }

    public string DisplayText => "reps ladder";
}

public enum ProgressiveOverloadType
{
    PowerLadder,
    RepsLadder,
    OneRepMaxPercentage,
    SteadyState
}

public enum ExerciseSelectorType
{
    SpecificExercise,
    ExerciseGroup,
    SpecificMuscle,
    MuscleGroup
}