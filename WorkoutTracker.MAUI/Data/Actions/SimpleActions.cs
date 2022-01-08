using System.Collections.Generic;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public record ReceiveExercisesAction(IEnumerable<Exercise> Exercises) : IAction;
    public record ReceiveExerciseLogsAction(IEnumerable<ExerciseLogEntry> ExerciseLogs) : IAction;
    public record ReceiveExerciseScheduleAction(IEnumerable<Exercise> Schedule) : IAction;
    public record AddExerciseLogEntryAction(ExerciseLogEntry Entry) : IAction;
    public record ReplaceExerciseRequest(Exercise ExerciseToReplace, IEnumerable<Exercise> CurrentSchedule, IEnumerable<Exercise> AllExercises);
    public record ForceStateChange() : IAction;
}
