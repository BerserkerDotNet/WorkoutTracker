using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Reducers
{
    public record ExerciseScheduleState(ScheduleViewModel[] Schedule, ExerciseProfile SelectedProfile, Guid? CurrentScheduleId);
    public class ExerciseScheduleReducer : IReducer<ExerciseScheduleState>
    {
        public ExerciseScheduleState Reduce(ExerciseScheduleState state, IAction action)
        {
            state = state ?? new ExerciseScheduleState(Array.Empty<ScheduleViewModel>(), ExerciseProfile.UpperBody, null);

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
                case SwapExerciseSchedulesAction a:
                    return MoveExerciseDown(state, a.ScheduleToSwap);
                case MoveExerciseDownAction a:
                    return MoveExerciseDown(state, a.ScheduleToSwap);
                case MoveExerciseUpAction a:
                    return MoveExerciseUp(state, a.ScheduleToSwap);
                case SetCurrentSchedule a:
                    return state with { CurrentScheduleId = a.Id };
                default:
                    return state;
            }
        }

        private ExerciseScheduleState MoveExerciseDown(ExerciseScheduleState state, ScheduleViewModel scheduleToSwap)
        {
            var currentExerciseIndex = Array.FindIndex(state.Schedule, e => e.Id == scheduleToSwap.Id);
            if (currentExerciseIndex == state.Schedule.Length - 1)
            {
                return state;
            }

            var swap = state.Schedule[currentExerciseIndex + 1];

            state.Schedule[currentExerciseIndex] = new ScheduleViewModel(scheduleToSwap.Id, swap.CurrentIndex, swap.TargetSets, swap.TargetRest, swap.Exercises);
            state.Schedule[currentExerciseIndex + 1] = new ScheduleViewModel(swap.Id, scheduleToSwap.CurrentIndex, scheduleToSwap.TargetSets, scheduleToSwap.TargetRest, scheduleToSwap.Exercises);

            return state;
        }

        private ExerciseScheduleState MoveExerciseUp(ExerciseScheduleState state, ScheduleViewModel scheduleToSwap)
        {
            var currentExerciseIndex = Array.FindIndex(state.Schedule, e => e.Id == scheduleToSwap.Id);
            if (currentExerciseIndex == 0)
            {
                return state;
            }

            var swap = state.Schedule[currentExerciseIndex - 1];

            state.Schedule[currentExerciseIndex] = new ScheduleViewModel(scheduleToSwap.Id, swap.CurrentIndex, swap.TargetSets, swap.TargetRest, swap.Exercises);
            state.Schedule[currentExerciseIndex - 1] = new ScheduleViewModel(swap.Id, scheduleToSwap.CurrentIndex, scheduleToSwap.TargetSets, scheduleToSwap.TargetRest, scheduleToSwap.Exercises);

            return state;
        }
    }
}
