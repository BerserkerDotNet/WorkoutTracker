using System;

namespace WorkoutTracker.Models
{
    [PluralName(EntityPluralNames.ExerciseLogEntryPluralName)]
    public class ExerciseLogEntry : EntityBase
    {
        public Guid ExerciseId { get; set; }

        public Set[] Sets { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
