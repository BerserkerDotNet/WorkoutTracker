using System.Collections.Generic;

namespace WorkoutTracker.MAUI.ViewModels
{
    public class ExerciseViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Steps { get; set; }

        public string TutorialUrl { get; set; }

        public string ImagePath { get; set; }

        public IEnumerable<MuscleViewModel> Muscles { get; set; }

        public string[] Tags { get; set; }
    }
}
