using System;
using WorkoutTracker.Models.Contracts;

namespace WorkoutTracker.Models.Entities;

public class LegacySet : IExerciseSet
{
    public double? WeightLB { get; set; }

    public double? WeightKG { get; set; }

    public int Repetitions { get; set; }

    public TimeSpan Duration { get; set; }

    public TimeSpan RestTime { get; set; }

    public DateTime CompletionTime { get; set; } = DateTime.UtcNow;

    public string Note { get; set; }

    public double Weight
    {
        get
        {
            return WeightLB ?? 0;
        }
        set
        {
            WeightLB = value;
            WeightKG = Math.Ceiling(value * 0.453592);
        }
    }
}

public sealed class ProposedSet : IExerciseSet
{
    public required double Weight { get; set; }

    public required int Repetitions { get; set; }
}

public sealed class InProgressSet : IExerciseSet
{
    public required double Weight { get; set; }

    public required int Repetitions { get; set; }

    public required TimeSpan RestTime { get; set; }
}

public sealed class CompletedSet : IExerciseSet
{
    public required double Weight { get; set; }

    public required int Repetitions { get; set; }

    public required TimeSpan RestTime { get; set; }

    public required TimeSpan Duration { get; set; }

    public required DateTime CompletionTime { get; set; }
}