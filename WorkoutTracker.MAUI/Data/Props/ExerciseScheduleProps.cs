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

    public class EditExerciseProps
    {
        public Exercise Exercise { get; set; }

        public EventCallback<ExerciseLogEntry> Save { get; set; }

        public EventCallback Cancel { get; set; }
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
