using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Reducers
{
    public record LogEntriesState(Dictionary<Guid, LogEntryViewModel> LastLogByExercise, Dictionary<DateOnly, IEnumerable<LogEntryViewModel>> History, DateOnly? SelectedDate);
    public class LogEntriesReducer : IReducer<LogEntriesState>
    {
        public LogEntriesState Reduce(LogEntriesState state, IAction action)
        {
            var today = DateOnly.FromDateTime(DateTime.Today.ToUniversalTime());
            state = state ?? new LogEntriesState(new Dictionary<Guid, LogEntryViewModel>(), new Dictionary<DateOnly, IEnumerable<LogEntryViewModel>>(), today);

            switch (action)
            {
                case ReceiveExerciseLogsAction a:
                    state.History[a.Date] = a.ExerciseLogs;
                    return state with { SelectedDate = a.Date };
                case ReceiveLastWorkoutLogByExerciseAction a:
                    state.LastLogByExercise[a.ExerciseId] = a.Entry;
                    return state;
                case AddExerciseLogEntryAction a:
                    var key = DateOnly.FromDateTime(a.Entry.Date);
                    var log = state.History.ContainsKey(key) ? state.History[key] : Enumerable.Empty<LogEntryViewModel>();
                    state.History[key] = log.Union(new[] { a.Entry }).OrderByDescending(e => e.Date);
                    return state;
                case SetSelectedHistoryDate a:
                    return state with { SelectedDate = a.Date };
                default:
                    return state;
            }
        }
    }
}
