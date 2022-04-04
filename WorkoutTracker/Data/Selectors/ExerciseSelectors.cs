using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.Data.Selectors;

public static class ExerciseSelectors 
{
    public static IEnumerable<ExerciseViewModel> SelectExercises(RootState state) 
    {
        return state.Exercises?.List ?? Enumerable.Empty<ExerciseViewModel>();
    }

    public static ExerciseViewModel SelectExerciseById(RootState state, Guid id)
    {
        return SelectExercises(state).FirstOrDefault(e => e.Id == id);
    }

    public static IEnumerable<string> SelectTags(RootState state)
    {
        return SelectExercises(state)
            .SelectMany(e => e.Tags)
            .Distinct();
    }
}
