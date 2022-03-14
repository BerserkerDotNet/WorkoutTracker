using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.ViewModels
{
    public record ScheduleViewModel(string Category, int CurrentIndex, IEnumerable<ExerciseViewModel> Exercises) 
    {
        public ExerciseViewModel CurrentExercise => Exercises.Count() > CurrentIndex ? Exercises.ElementAt(CurrentIndex) : null;
        public ExerciseWithCategoryViewModel CurrentExerciseWithCategory => new ExerciseWithCategoryViewModel(Category, CurrentExercise);
    }

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
