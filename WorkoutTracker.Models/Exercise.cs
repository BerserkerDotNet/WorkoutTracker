using System;

namespace WorkoutTracker.Models
{
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
}
