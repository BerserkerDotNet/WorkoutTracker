using BlazorState.Redux.Utilities;

namespace WorkoutTracker.Data.Props
{
    public record PreviousLogRecordStats(WorkoutSummary BestWorkout, WorkoutSummary LastWorkout)
    {
        public string MaxWeightFormatted => $"{BestWorkout.Max.WeightLb} LB";

        public string LastWeightFormatted => $"{LastWorkout.Max.WeightLb} LB";
    }

    public class ExerciseScheduleProps
    {
        public IEnumerable<WorkoutViewModel> Schedule { get; set; }

        public IEnumerable<ExerciseViewModel> AllExercises { get; set; }

        public AsyncAction<IExerciseSelector> AddExercise { get; set; }
    }

    public class ExerciseSchedulePanelProps
    {
        public Dictionary<Guid, PreviousLogRecordStats> PreviousSessionLog { get; set; }
    }

    public record ExerciseActionBarProps(IEnumerable<ExerciseViewModel> AllExercises, Action<Guid> IncreaseSets, Action<Guid> DecreaseSets, Action<Guid, ExerciseViewModel> ReplaceExercise, AsyncAction<Guid> RemoveExercise);

    public record ExerciseSetsProps(Dictionary<Guid, LogEntryViewModel> TodayLogByExercise, Action<Guid, int> StartSet, Action<Guid, int> FinishSet, Action<Guid, WorkoutExerciseSetViewModel> UpdateSet, AsyncAction<LogEntryViewModel> Save);

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
