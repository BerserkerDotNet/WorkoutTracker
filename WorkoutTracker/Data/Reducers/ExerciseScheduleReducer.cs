using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Reducers
{
    public record ExerciseScheduleState(WorkoutViewModel[] WorkoutSchedule, ExerciseProfile SelectedProfile);
    public class ExerciseScheduleReducer : IReducer<ExerciseScheduleState>
    {
        public ExerciseScheduleState Reduce(ExerciseScheduleState state, IAction action)
        {
            state = state ?? new ExerciseScheduleState(Array.Empty<WorkoutViewModel>(), ExerciseProfile.GetDefaultProfile());

            switch (action)
            {
                case ExerciseProfileSelected a:
                    return state with { SelectedProfile = a.Profile };
                case ReceiveExerciseScheduleAction a:
                    return state with { WorkoutSchedule = a.Schedule };
                case SwapExerciseSchedulesAction a:
                    return MoveExerciseDown(state, a.ScheduleToSwap);
                case MoveExerciseDownAction a:
                    return MoveExerciseDown(state, a.ScheduleToSwap);
                case MoveExerciseUpAction a:
                    return MoveExerciseUp(state, a.ScheduleToSwap);
                case ReplaceScheduleExercise si:
                    var scheduleToUpdate = Array.FindIndex(state.WorkoutSchedule, s => s.Id == si.ScheduleId);
                    state.WorkoutSchedule[scheduleToUpdate] = state.WorkoutSchedule[scheduleToUpdate] with { Exercise = si.NewExercise };
                    return state;
                case UpdateSetStatus a:
                    var scheduleIndexToUpdate = Array.FindIndex(state.WorkoutSchedule, s => s.Id == a.ScheduleId);
                    var wSchedule = state.WorkoutSchedule[scheduleIndexToUpdate];
                    var setsArray = wSchedule.Exercise.Sets.ToArray();
                    setsArray[a.SetIndex] = setsArray[a.SetIndex] with { Status = a.Status };
                    var newExercise = wSchedule.Exercise with { Sets = setsArray };
                    var newSchedule = wSchedule with { Exercise = newExercise };
                    state.WorkoutSchedule[scheduleIndexToUpdate] = newSchedule;
                    return state;
                case UpdateSet a:
                    return UpdateSet(state, a.ScheduleId, a.Set);
                case IncreaseSets a:
                    return IncreaseSets(state, a.ScheduleId);
                case DecreaseSets a:
                    return DecreaseSets(state, a.ScheduleId);
                default:
                    return state;
            }
        }

        private ExerciseScheduleState UpdateSet(ExerciseScheduleState state, Guid ScheduleId, WorkoutExerciseSetViewModel set)
        {
            var scheduleIndexToUpdate = Array.FindIndex(state.WorkoutSchedule, s => s.Id == ScheduleId);
            var wSchedule = state.WorkoutSchedule[scheduleIndexToUpdate];
            var setsArray = wSchedule.Exercise.Sets.ToArray();
            setsArray[set.Index] = setsArray[set.Index] = set;
            var newExercise = wSchedule.Exercise with { Sets = setsArray };
            var newSchedule = wSchedule with { Exercise = newExercise };
            state.WorkoutSchedule[scheduleIndexToUpdate] = newSchedule;
            return state;
        }

        private ExerciseScheduleState IncreaseSets(ExerciseScheduleState state, Guid ScheduleId)
        {
            var scheduleToIncreaseSets = Array.FindIndex(state.WorkoutSchedule, s => s.Id == ScheduleId);
            var scheduleToUpdate = state.WorkoutSchedule[scheduleToIncreaseSets];
            var setsList = scheduleToUpdate.Exercise.Sets.ToList();
            setsList.Add(new WorkoutExerciseSetViewModel(setsList.Count(), SetStatus.NotStarted, 0, 0, TimeSpan.Zero, TimeSpan.Zero));
            var newExercise = scheduleToUpdate.Exercise with { Sets = setsList };
            var newSchedule = scheduleToUpdate with { Exercise = newExercise };
            state.WorkoutSchedule[scheduleToIncreaseSets] = newSchedule;
            return state;
        }

        private ExerciseScheduleState DecreaseSets(ExerciseScheduleState state, Guid ScheduleId)
        {
            var scheduleToIncreaseSets = Array.FindIndex(state.WorkoutSchedule, s => s.Id == ScheduleId);
            var scheduleToUpdate = state.WorkoutSchedule[scheduleToIncreaseSets];
            var setsList = scheduleToUpdate.Exercise.Sets.ToList();
            var canDecrease = setsList.Last().Status != SetStatus.Completed;
            if (canDecrease)
            {
                setsList.RemoveAt(setsList.Count - 1);
            }

            var newExercise = scheduleToUpdate.Exercise with { Sets = setsList };
            var newSchedule = scheduleToUpdate with { Exercise = newExercise };
            state.WorkoutSchedule[scheduleToIncreaseSets] = newSchedule;
            return state;
        }

        private ExerciseScheduleState MoveExerciseDown(ExerciseScheduleState state, WorkoutViewModel scheduleToSwap)
        {
            var currentExerciseIndex = Array.FindIndex(state.WorkoutSchedule, e => e.Id == scheduleToSwap.Id);
            if (currentExerciseIndex == state.WorkoutSchedule.Length - 1)
            {
                return state;
            }

            var swap = state.WorkoutSchedule[currentExerciseIndex + 1];

            state.WorkoutSchedule[currentExerciseIndex] = new WorkoutViewModel(scheduleToSwap.Id, swap.TargetRestTime, swap.Exercise);
            state.WorkoutSchedule[currentExerciseIndex + 1] = new WorkoutViewModel(swap.Id, scheduleToSwap.TargetRestTime, scheduleToSwap.Exercise);

            return state;
        }

        private ExerciseScheduleState MoveExerciseUp(ExerciseScheduleState state, WorkoutViewModel scheduleToSwap)
        {
            var currentExerciseIndex = Array.FindIndex(state.WorkoutSchedule, e => e.Id == scheduleToSwap.Id);
            if (currentExerciseIndex == 0)
            {
                return state;
            }

            var swap = state.WorkoutSchedule[currentExerciseIndex - 1];

            state.WorkoutSchedule[currentExerciseIndex] = new WorkoutViewModel(scheduleToSwap.Id, swap.TargetRestTime, swap.Exercise);
            state.WorkoutSchedule[currentExerciseIndex - 1] = new WorkoutViewModel(swap.Id, scheduleToSwap.TargetRestTime, scheduleToSwap.Exercise);

            return state;
        }
    }
}
