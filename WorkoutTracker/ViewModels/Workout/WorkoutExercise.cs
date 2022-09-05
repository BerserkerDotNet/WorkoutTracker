using WorkoutTracker.Models;

namespace WorkoutTracker.ViewModels.Workout;

public class WorkoutExercise
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Steps { get; set; }

    public string TutorialUrl { get; set; }

    public string ImagePath { get; set; }

    public IEnumerable<Muscle> Muscles { get; set; }

    public IEnumerable<string> Tags { get; set; }

    public IList<WorkoutSet> Sets { get; set; }
}
