using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Reducers
{
    public record ExercisesState(IEnumerable<ExerciseViewModel> List);

    public class ExercisesReducer : IReducer<ExercisesState>
    {
        public ExercisesState Reduce(ExercisesState state, IAction action)
        {
            state = state ?? new ExercisesState(Enumerable.Empty<ExerciseViewModel>());

            switch (action)
            {
                case ReceiveExercisesAction a:
                    return state with { List = a.Exercises };
                default:
                    return state;
            }
        }
    }
}
