using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.MAUI.Data.Actions;

namespace WorkoutTracker.MAUI.Data.Reducers
{
    public record LogEntriesState(Dictionary<DateOnly, IEnumerable<LogEntryViewModel>> History);
    public class LogEntriesReducer : IReducer<LogEntriesState>
    {
        public LogEntriesState Reduce(LogEntriesState state, IAction action)
        {
            state = state ?? new LogEntriesState(new Dictionary<DateOnly, IEnumerable<LogEntryViewModel>>());

            switch (action)
            {
                case ReceiveExerciseLogsAction a:
                    state.History[a.Date] = a.ExerciseLogs;
                    return state;
                case AddExerciseLogEntryAction a:
                    var key = DateOnly.FromDateTime(a.Entry.Date);
                    var log = state.History.ContainsKey(key) ? state.History[key] : Enumerable.Empty<LogEntryViewModel>();
                    state.History[key] = log.Union(new[] { a.Entry }).OrderByDescending(e => e.Date);
                    return state;
                default:
                    return state;
            }
        }
    }
}
