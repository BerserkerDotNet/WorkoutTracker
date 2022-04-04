using System.Collections.Generic;

namespace WorkoutTracker.Data.Actions
{
    public record ReceiveExercisesAction(IEnumerable<ExerciseViewModel> Exercises) : IAction;
    public record ReceiveMusclesAction(IEnumerable<MuscleViewModel> Muscles) : IAction;
    public record ReceiveExerciseLogsAction(DateOnly Date, IEnumerable<LogEntryViewModel> ExerciseLogs) : IAction;
    public record ReceiveExerciseScheduleAction(Dictionary<string, ScheduleViewModel> Schedule) : IAction;
    public record ReceiveNewExerciseScheduleItemAction(ScheduleViewModel ScheduleItem) : IAction;
    public record AddExerciseLogEntryAction(LogEntryViewModel Entry) : IAction;
    public record ReceiveLastWorkoutLogByExerciseAction(Guid ExerciseId, LogEntryViewModel Entry) : IAction;
    public record ForceStateChange() : IAction;
}
