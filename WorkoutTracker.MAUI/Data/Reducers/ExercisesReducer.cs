using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.MAUI.Data.Actions;
using WorkoutTracker.MAUI.ViewModels;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Reducers
{
    public record ExercisesState(Dictionary<Guid, ExerciseViewModel> List, IEnumerable<LogEntryViewModel> Log, IEnumerable<ExerciseViewModel> Schedule);

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
                default:
                    return state;
            }
        }
    }
}
