using System.Collections.Generic;
using WorkoutTracker.Models;

namespace WorkoutTracker.ViewModels
{
    public class LogEntryViewModel
    {
        public Guid Id { get; set; }

        public ExerciseViewModel Exercise { get; set; }

        public IEnumerable<Set> Sets { get; set; }

        public DateTime Date { get; set; }
    }
}
