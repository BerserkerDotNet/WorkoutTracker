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

        public double TotalWeightKG => Math.Ceiling(Sets.Sum(s => (s.WeightKG ?? 0) * s.Repetitions));

        public double TotalWeightLB => Math.Ceiling(Sets.Sum(s => (s.WeightLB ?? 0) * s.Repetitions));
    }
}
