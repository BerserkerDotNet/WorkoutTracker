using System;

namespace WorkoutTracker.Models
{
    [PluralName(EndpointNames.ExerciseLogEntryPluralName)]
    public class ExerciseLogEntry : EntityBase
    {
        public Guid ExerciseId { get; set; }

        public Set[] Sets { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
