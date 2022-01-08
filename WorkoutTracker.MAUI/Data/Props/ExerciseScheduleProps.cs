using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Props
{
    public class ExerciseScheduleProps
    {
        public IEnumerable<Exercise> Schedule { get; set; }

        public Dictionary<Guid, int> ExerciseCount { get; set; }

        public EventCallback<Guid> Start { get; set; }

        public EventCallback<Exercise> Replace { get; set; }

        public EventCallback Rebuild { get; set; }

        public EventCallback OpenLog { get; set; }
    }

    public class ExercisesLogProps
    {
        public ICollection<LogRecord> Log { get; set; }

        public EventCallback<Guid> Delete { get; set; }

        public EventCallback<ExerciseLogEntry> Edit { get; set; }
    }

    public class EditExerciseLogProps
    {
        public Exercise Exercise { get; set; }

        public Guid? NextExerciseId { get; set; }

        public Guid? PreviousExerciseId { get; set; }

        public int ExerciseSetNumber { get; set; }

		public IEnumerable<ExerciseLogEntry> Sets { get; set; }

		public EventCallback<ExerciseLogEntry> Save { get; set; }

        public EventCallback<Guid> Next { get; set; }

        public EventCallback<Guid> Previous { get; set; }

        public EventCallback Cancel { get; set; }
    }

    public class EditExerciseProps
    {
        public Exercise Exercise { get; set; }

        public EventCallback<Exercise> Save { get; set; }

        public EventCallback Cancel { get; set; }
    }

    public class ExerciseListProps
    {
        public IEnumerable<Exercise> List { get; set; }

        public EventCallback<Exercise> Edit { get; set; }
    }

    public class LogRecord
    {
        public LogRecord(Exercise exercise, ExerciseLogEntry entry)
        {
            Exercise = exercise;
            Entry = entry;
        }

        public Exercise Exercise { get; }
        public ExerciseLogEntry Entry { get; }
    }
}
