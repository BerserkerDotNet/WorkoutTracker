using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Reducers
{
    public record ExerciseLogUIState(DateOnly SelectedDate, Guid? SelectedItemId = null);
    public record LogEntriesState(Dictionary<Guid, LogEntryViewModel> LastLogByExercise, Dictionary<DateOnly, IEnumerable<LogEntryViewModel>> History, ExerciseLogUIState UIState);
    public class LogEntriesReducer : IReducer<LogEntriesState>
    {
        public LogEntriesState Reduce(LogEntriesState state, IAction action)
        {
            var today = DateOnly.FromDateTime(DateTime.Today.ToUniversalTime());
            state = state ?? new LogEntriesState(new Dictionary<Guid, LogEntryViewModel>(), new Dictionary<DateOnly, IEnumerable<LogEntryViewModel>>(), new ExerciseLogUIState(today));

            switch (action)
            {
                case ReceiveExerciseLogsAction a:
                    state.History[a.Date] = a.ExerciseLogs;
                    var newUiState = state.UIState with { SelectedDate = a.Date };
                    return state with { UIState = newUiState };
                case ReceiveLastWorkoutLogByExerciseAction a:
                    state.LastLogByExercise[a.ExerciseId] = a.Entry;
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
