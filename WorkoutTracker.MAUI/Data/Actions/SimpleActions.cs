using System.Collections.Generic;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public record ReceiveExercisesAction(IEnumerable<ExerciseViewModel> Exercises) : IAction;
    public record ReceiveExerciseLogsAction(IEnumerable<LogEntryViewModel> ExerciseLogs) : IAction;
    public record ReceiveExerciseScheduleAction(Dictionary<string, ScheduleViewModel> Schedule) : IAction;
    public record ReceiveNewExerciseScheduleItemAction(ScheduleViewModel ScheduleItem) : IAction;
    public record AddExerciseLogEntryAction(LogEntryViewModel Entry) : IAction;
    public record ForceStateChange() : IAction;
}
