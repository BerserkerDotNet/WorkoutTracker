using System;

namespace WorkoutTracker.Models
{
    [PluralName("ExerciseLogEntries")]
    public class ExerciseLogEntry : EntityBase
    {
        public Guid ExerciseId { get; set; }

        public Set[] Sets { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

    public class Set
    {
        public double? Weight { get; set; }

        public int Repetitions { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan RestTime { get; set; }

        public DateTime CompletionTime { get; set; } = DateTime.UtcNow;

        public string Note { get; set; }
    }
}
