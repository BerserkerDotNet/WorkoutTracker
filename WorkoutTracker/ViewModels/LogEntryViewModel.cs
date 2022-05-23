using WorkoutTracker.Models;

namespace WorkoutTracker.ViewModels
{
    public class LogEntryViewModel
    {
        public Guid Id { get; set; }

        public ExerciseViewModel Exercise { get; set; }

        public IEnumerable<Set> Sets { get; set; }

        public DateTime Date { get; set; }

        public double TotalDuration => Math.Ceiling(Sets.Sum(s => s.Duration.TotalMinutes));

        public double TotalRest => Math.Ceiling(Sets.Sum(s => s.RestTime.TotalMinutes));

        public double TotalWeight => Math.Ceiling(Sets.Sum(s => s.Weight ?? 0));

       
    }
}
