using System;

namespace WorkoutTracker.Models
{
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
}
