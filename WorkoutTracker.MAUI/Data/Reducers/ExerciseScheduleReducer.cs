using System.Collections.Generic;
using WorkoutTracker.MAUI.Data.Actions;

namespace WorkoutTracker.MAUI.Data.Reducers
{
    public record ExerciseScheduleState(Dictionary<string, ScheduleViewModel> Schedule);
    public class ExerciseScheduleReducer : IReducer<ExerciseScheduleState>
    {
        public ExerciseScheduleState Reduce(ExerciseScheduleState state, IAction action)
        {
            state = state ?? new ExerciseScheduleState(new Dictionary<string, ScheduleViewModel>());

            switch (action)
            {
                case ReceiveExerciseScheduleAction a:
                    return state with { Schedule = a.Schedule };
                case ReceiveNewExerciseScheduleItemAction si:
                    state.Schedule[si.ScheduleItem.Category] = si.ScheduleItem;
                    return state;
                default:
                    return state;
            }
        }
    }
}
