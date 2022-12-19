using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Data.Actions;

public record ExerciseProfileSelected(ExerciseProfile Profile) : IAction;
public record ReceiveExercisesAction(IEnumerable<ExerciseViewModel> Exercises) : IAction;
public record ReceiveWorkoutProgramsAction(IEnumerable<WorkoutProgram> Programs) : IAction;
public record ReceiveMusclesAction(IEnumerable<MuscleViewModel> Muscles) : IAction;
public record ReceiveExerciseLogsAction(DateOnly Date, IEnumerable<LogEntryViewModel> ExerciseLogs) : IAction;
public record ReceiveWorkoutStatsAction(IEnumerable<WorkoutSummary> Summaries) : IAction;
public record ReceiveExerciseScheduleAction(WorkoutViewModel[] Schedule) : IAction;
public record SwapExerciseSchedulesAction(WorkoutViewModel ScheduleToSwap) : IAction;
public record MoveExerciseUpAction(WorkoutViewModel ScheduleToSwap) : IAction;
public record MoveExerciseDownAction(WorkoutViewModel ScheduleToSwap) : IAction;
public record ReplaceScheduleExercise(Guid ScheduleId, WorkoutExerciseViewModel NewExercise) : IAction;
public record UpsertExerciseLogEntryAction(LogEntryViewModel Entry) : IAction;
public record ReceiveLastWorkoutLogByExerciseAction(Guid ExerciseId, LogEntryViewModel Entry) : IAction;
public record ForceStateChange() : IAction;
public record ExercisesFilterChanged(ExercisesFilterViewModel Filter) : IAction;
public record SetSelectedHistoryDate(DateOnly Date) : IAction;
public record ShowProgressIndicator(string Text) : IAction;
public record HideProgressIndicator() : IAction;
public record UpdateSetStatus(Guid ScheduleId, int SetIndex, SetStatus Status) : IAction;
public record UpdateSet(Guid ScheduleId, WorkoutExerciseSetViewModel Set) : IAction;
public record IncreaseSets(Guid ScheduleId) : IAction;
public record DecreaseSets(Guid ScheduleId) : IAction;