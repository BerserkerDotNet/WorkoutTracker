using System.Collections.Generic;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public record ReceiveExercisesAction(IEnumerable<ExerciseViewModel> Exercises) : IAction;
    public record ReceiveExerciseLogsAction(IEnumerable<LogEntryViewModel> ExerciseLogs) : IAction;
    public record ReceiveExerciseScheduleAction(IEnumerable<ExerciseViewModel> Schedule) : IAction;
    public record AddExerciseLogEntryAction(LogEntryViewModel Entry) : IAction;
    public record ReplaceExerciseRequest(ExerciseViewModel ExerciseToReplace, IEnumerable<ExerciseViewModel> CurrentSchedule, IEnumerable<ExerciseViewModel> AllExercises);
    public record ForceStateChange() : IAction;
}
