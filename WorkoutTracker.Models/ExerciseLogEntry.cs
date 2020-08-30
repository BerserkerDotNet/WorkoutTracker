using System;

namespace WorkoutTracker.Models
{
    public class ExerciseLogEntry : EntityBase
    {
        public Guid ExerciseId { get; set; }

        public double? Weight { get; set; }

        public int Repetitions { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public int Score { get; set; }

        public string Note { get; set; }
    }
}
