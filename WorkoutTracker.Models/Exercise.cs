using System;

namespace WorkoutTracker.Models
{
    [PluralName("Exercises")]
    public class Exercise : EntityBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Steps { get; set; }

        public string TutorialUrl { get; set; }

        public byte[] Icon { get; set; }

        public Guid[] Muscles { get; set; }

        public string[] Tags { get; set; }
    }
}
