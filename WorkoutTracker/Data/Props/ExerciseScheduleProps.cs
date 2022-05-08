using BlazorState.Redux.Utilities;
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
        public IEnumerable<ScheduleViewModel> Schedule { get; set; }

        public Action<IEnumerable<ScheduleViewModel>> Start { get; set; }

        public Action<ScheduleViewModel> Previous { get; set; }

        public Action<ScheduleViewModel> Next { get; set; }
    }

    public class ExercisesLogProps
    {
        public Dictionary<DateOnly, IEnumerable<LogEntryViewModel>> Log { get; set; }

        public DateOnly SelectedDate { get; set; }

        public EventCallback<Guid> Delete { get; set; }

        public EventCallback<LogEntryViewModel> Save { get; set; }

        public EventCallback<DateTime> Load { get; set; }
    }

    public class EditExerciseProps
    {
        public EditExerciseViewModel Exercise { get; set; }

        public IEnumerable<MuscleViewModel> Muscles { get; set; }

        public IEnumerable<string> Tags { get; set; }

        public bool IsLoading => !Muscles.Any();

        public AsyncAction<EditExerciseViewModel> Save { get; set; }

        public Action Cancel { get; set; }
    }

    public class ExercisesListProps
    {
        public ICollection<ExerciseViewModel> List { get; set; }

        public Action<ExerciseViewModel> Edit { get; set; }

        public AsyncAction<Guid> Delete { get; set; }
    }

    public class ExercisesFilterProps
    {
        public IEnumerable<string> MuscleGroups { get; set; }

        public ExercisesFilterViewModel Filter { get; set; }

        public Action Add { get; set; }
     
        public Action<ExercisesFilterViewModel> ApplyFilter { get; set; }
    }
}
