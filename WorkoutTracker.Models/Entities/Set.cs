using System;
using System.Text.Json;

namespace WorkoutTracker.Models.Entities;

public class Set
{
    public double? WeightLB { get; set; }

    public double? WeightKG { get; set; }

    public int Repetitions { get; set; }

    public TimeSpan Duration { get; set; }

    public TimeSpan RestTime { get; set; }

    public DateTime CompletionTime { get; set; } = DateTime.UtcNow;

    public string Note { get; set; }
}

public class WorkoutSetLog
{
    public JsonElement VariantData { get; set; }

    public SetVariant VariantType { get; set; }

    public TimeSpan Duration { get; set; }

    public TimeSpan RestTime { get; set; }

    public DateTime CompletionTime { get; set; } = DateTime.UtcNow;

}

public class RepetitionsSetVariant
{
    public int Repetitions { get; set; }

    public double WeightInPounds { get; set; }
}

public class TimeSetVariant
{
    public double Time { get; set; }
}

public enum SetVariant
{
    Repetitions,
    Time
}
