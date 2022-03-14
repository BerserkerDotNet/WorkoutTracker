using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using UnitsNet;

namespace WorkoutTracker.Data.Props
{
    public record ExerciseWithCategoryViewModel(string Category, ExerciseViewModel Exercise);

    public record PreviousLogRecordStats(double? Weight, int Repetitions) 
    {
        public double WeightInLB => Weight.HasValue ? Math.Floor(Mass.FromKilograms(Weight.Value).Pounds) : 0.0d;
    } 

    public class ExerciseScheduleProps
    {
        public IEnumerable<ExerciseWithCategoryViewModel> Schedule { get; set; }

        public Dictionary<Guid, int> ExerciseCountLookup { get; set; }

        public EventCallback<ExerciseWithCategoryViewModel> Start { get; set; }

        public EventCallback<string> Previous { get; set; }

        public EventCallback<string> Next { get; set; }

        public EventCallback<ExerciseProfile> Rebuild { get; set; }

        public EventCallback OpenLog { get; set; }
    }

    public class ExercisesLogProps
    {
        public Dictionary<DateOnly, IEnumerable<LogEntryViewModel>> Log { get; set; }

        public DateOnly SelectedDate { get; set; }

        public EventCallback<Guid> Delete { get; set; }

        public EventCallback<LogEntryViewModel> Save { get; set; }

        public EventCallback<DateTime> Load { get; set; }
    }

    public class EditExerciseLogProps
    {
        public ExerciseWithCategoryViewModel NextExerciseId { get; set; }

        public ExerciseWithCategoryViewModel PreviousExerciseId { get; set; }

        public int SetNumber { get; set; }

		public LogEntryViewModel Log { get; set; }

        public PreviousLogRecordStats PreviousLog { get; set; }

        public bool PreviousLogLoading { get; set; }

        public EventCallback<LogEntryViewModel> Save { get; set; }

        public EventCallback<ExerciseWithCategoryViewModel> Next { get; set; }

        public EventCallback<ExerciseWithCategoryViewModel> Previous { get; set; }

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
        public ICollection<ExerciseViewModel> List { get; set; }

        public EventCallback<ExerciseViewModel> Edit { get; set; }
    }
}
