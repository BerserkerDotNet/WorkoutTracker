using BlazorState.Redux.Utilities;
using Microsoft.AspNetCore.Components;

namespace WorkoutTracker.Data.Props
{
    public record PreviousLogRecordStats(WorkoutSummary BestWorkout, WorkoutSummary LastWorkout)
    {
        public string MaxWeightFormatted => $"{BestWorkout.Max.WeightLb} LB";

        public string LastWeightFormatted => $"{LastWorkout.Max.WeightLb} LB";
    }

    public class ExerciseScheduleProps
    {
        public Guid? CurrentScheduleId { get; set; }

        public IEnumerable<ScheduleViewModel> Schedule { get; set; }

        public Dictionary<Guid, LogEntryViewModel> TodayLogByExercise { get; set; }

        public Dictionary<Guid, PreviousLogRecordStats> PreviousSessionLog { get; set; }

        public IEnumerable<ExerciseViewModel> AllExercises { get; set; }

        public AsyncAction<IExerciseSelector> AddExercise { get; set; }

        public AsyncAction<LogEntryViewModel> Save { get; set; }

        public AsyncAction<Guid> RemoveExercise { get; set; }

        public Action<Guid, int> SetScheduleTargetSets { get; set; }
    }

    public record ExerciseControlContext(AsyncAction<IExerciseSelector> AddExercise, AsyncAction<Guid> RemoveExercise, AsyncAction<LogEntryViewModel> SaveExercise, Action<Guid, int> SetScheduleTargetSets);

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
