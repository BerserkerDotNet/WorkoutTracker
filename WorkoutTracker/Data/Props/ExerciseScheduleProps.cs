using Microsoft.AspNetCore.Components;
using UnitsNet;

namespace WorkoutTracker.Data.Props
{
    public record PreviousLogRecordStats(double? Weight, int Repetitions) 
    {
        public double WeightInLB => Weight.HasValue ? Math.Floor(Mass.FromKilograms(Weight.Value).Pounds) : 0.0d;
    } 

    public class ExerciseScheduleProps
    {
        public ExerciseProfile SelectedProfile { get; set; }

        public IEnumerable<ScheduleViewModel> Schedule { get; set; }

        public Dictionary<Guid, int> ExerciseCountLookup { get; set; }

        public EventCallback<ScheduleViewModel> Start { get; set; }

        public EventCallback<ScheduleViewModel> Previous { get; set; }

        public EventCallback<ScheduleViewModel> Next { get; set; }

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
        public ScheduleViewModel NextExerciseId { get; set; }

        public ScheduleViewModel PreviousExerciseId { get; set; }

        public int SetNumber { get; set; }

		public LogEntryViewModel Log { get; set; }

        public PreviousLogRecordStats PreviousLog { get; set; }

        public bool PreviousLogLoading { get; set; }

        public EventCallback<LogEntryViewModel> Save { get; set; }

        public EventCallback<ScheduleViewModel> Next { get; set; }

        public EventCallback<ScheduleViewModel> Previous { get; set; }

        public EventCallback Cancel { get; set; }
    }

    public class EditExerciseProps
    {
        public EditExerciseViewModel Exercise { get; set; }

        public IEnumerable<MuscleViewModel> Muscles { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public bool IsLoading => !Muscles.Any();

        public EventCallback<EditExerciseViewModel> Save { get; set; }

        public EventCallback Cancel { get; set; }
    }

    public class ExerciseListProps
    {
        public IEnumerable<string> MuscleGroups { get; set; }

        public ICollection<ExerciseViewModel> List { get; set; }

        public EventCallback<ExerciseViewModel> Edit { get; set; }

        public EventCallback<Guid> Delete { get; set; }

        public EventCallback Add { get; set; }
    }
}
