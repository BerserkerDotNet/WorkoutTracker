using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Reducers;

public record ExercisesState(IEnumerable<ExerciseViewModel> List, IEnumerable<MuscleViewModel> Muscles, ExercisesFilterViewModel Filter);

public class ExercisesReducer : IReducer<ExercisesState>
{
    public ExercisesState Reduce(ExercisesState state, IAction action)
    {
        state = state ?? new ExercisesState(Enumerable.Empty<ExerciseViewModel>(), Enumerable.Empty<MuscleViewModel>(), ExercisesFilterViewModel.Empty);

        switch (action)
        {
            case ReceiveExercisesAction a:
                return state with { List = a.Exercises };
            case ReceiveMusclesAction a:
                return state with { Muscles = a.Muscles };
            case ExercisesFilterChanged a:
                return state with { Filter = a.Filter };
            default:
                return state;
        }
    }
}
