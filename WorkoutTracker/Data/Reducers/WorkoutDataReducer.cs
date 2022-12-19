using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Reducers;

public class WorkoutDataReducer : IReducer<WorkoutDataState>
{
    public WorkoutDataState Reduce(WorkoutDataState state, IAction action)
    {
        state = state ?? new WorkoutDataState(null);

        switch (action)
        {
            case ReceiveWorkoutProgramsAction a:
                return state with { WorkoutPrograms = a.Programs };
            default:
                return state;
        }
    }
}