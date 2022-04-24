using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Reducers
{
    public record ExerciseScheduleState(ScheduleViewModel[] Schedule, ExerciseProfile SelectedProfile);
    public class ExerciseScheduleReducer : IReducer<ExerciseScheduleState>
    {
        public ExerciseScheduleState Reduce(ExerciseScheduleState state, IAction action)
        {
            state = state ?? new ExerciseScheduleState(Array.Empty<ScheduleViewModel>(), ExerciseProfile.UpperBody);

            switch (action)
            {
                case ExerciseProfileSelected a:
                    return state with { SelectedProfile = a.Profile };
                case ReceiveExerciseScheduleAction a:
                    return state with { Schedule = a.Schedule };
                case ReceiveExerciseCurrentIndexAction si:
                    var idx = Array.FindIndex(state.Schedule, s => s.Id == si.ExerciseGroupId);
                    var item = state.Schedule[idx];
                    state.Schedule[idx] = item with { CurrentIndex = si.Index };
                    return state;
                default:
                    return state;
            }
        }
    }
}
