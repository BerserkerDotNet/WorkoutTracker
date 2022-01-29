using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.MAUI.Data.Actions;

namespace WorkoutTracker.MAUI.Data.Reducers
{
    public record ExercisesState(Dictionary<Guid, ExerciseViewModel> List, IEnumerable<LogEntryViewModel> Log, Dictionary<string, ScheduleViewModel> Schedule);

    public class ExercisesReducer : IReducer<ExercisesState>
    {
        public ExercisesState Reduce(ExercisesState state, IAction action)
        {
            state = state ?? new ExercisesState(null, null, null);

            switch (action)
            {
                case ReceiveExercisesAction a:
                    return state with { List = a.Exercises.ToDictionary(k => k.Id) };
                case ReceiveExerciseLogsAction a:
                    return state with { Log = a.ExerciseLogs };
                case ReceiveExerciseScheduleAction a:
                    return state with { Schedule = a.Schedule };
                case AddExerciseLogEntryAction a:
                    var log = state.Log ?? Enumerable.Empty<LogEntryViewModel>();
                    return state with { Log = new[] { a.Entry }.Union(log)};
                case ReceiveNewExerciseScheduleItemAction si:
                    state.Schedule[si.ScheduleItem.Category] = si.ScheduleItem; // This is mutating schedule, re-create instead?
                    return state with { Schedule = state.Schedule };
                default:
                    return state;
            }
        }
    }
}
