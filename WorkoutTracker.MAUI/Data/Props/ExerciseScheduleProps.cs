using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace WorkoutTracker.MAUI.Data.Props
{
    public class ExerciseScheduleProps
    {
        public IEnumerable<ExerciseViewModel> Schedule { get; set; }

        public Dictionary<Guid, int> ExerciseCount { get; set; }

        public EventCallback<Guid> Start { get; set; }

        public EventCallback<ExerciseViewModel> Replace { get; set; }

        public EventCallback Rebuild { get; set; }

        public EventCallback OpenLog { get; set; }
    }

    public class ExercisesLogProps
    {
        public ICollection<LogEntryViewModel> Log { get; set; }

        public EventCallback<Guid> Delete { get; set; }

        public EventCallback<LogEntryViewModel> Edit { get; set; }
    }

    public class EditExerciseLogProps
    {
        public Guid? NextExerciseId { get; set; }

        public Guid? PreviousExerciseId { get; set; }

        public int SetNumber { get; set; }

		public LogEntryViewModel Log { get; set; }

		public EventCallback<LogEntryViewModel> Save { get; set; }

        public EventCallback<Guid> Next { get; set; }

        public EventCallback<Guid> Previous { get; set; }

        public EventCallback Cancel { get; set; }
    }

    public class EditExerciseProps
    {
        public ExerciseViewModel Exercise { get; set; }

        public EventCallback<ExerciseViewModel> Save { get; set; }

        public EventCallback Cancel { get; set; }
    }

    public class ExerciseListProps
    {
        public IEnumerable<ExerciseViewModel> List { get; set; }

        public EventCallback<ExerciseViewModel> Edit { get; set; }
    }
}
