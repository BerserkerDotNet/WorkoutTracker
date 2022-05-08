using Microsoft.AspNetCore.Components.Forms;

namespace WorkoutTracker.ViewModels
{
    public record ExercisesFilterViewModel(string Name, IEnumerable<string> MuscleGroups) 
    {
        public static ExercisesFilterViewModel Empty = new ExercisesFilterViewModel(string.Empty, Enumerable.Empty<string>());
    }

    public record ScheduleViewModel(Guid Id, int CurrentIndex, int TargetSets, TimeSpan TargetRest, IEnumerable<ExerciseViewModel> Exercises) 
    {
        public int TargetSets { get; set; } = TargetSets;
        public TimeSpan TargetRest { get; set; } = TargetRest;

        public ExerciseViewModel CurrentExercise => Exercises.Count() > CurrentIndex ? Exercises.ElementAt(CurrentIndex) : null;
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

        public IEnumerable<string> Tags { get; set; }
    }

    public class EditExerciseViewModel : ExerciseViewModel 
    {
        public IBrowserFile ImageFile { get; set; }
    }
}
