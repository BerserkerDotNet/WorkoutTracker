namespace WorkoutTracker.Data.Selectors;

public static class ExerciseSelectors 
{
    public static IEnumerable<ExerciseViewModel> SelectExercises(this RootState state) 
    {
        return state.Exercises?.List ?? Enumerable.Empty<ExerciseViewModel>();
    }

    public static ExerciseViewModel SelectExerciseById(this RootState state, Guid id)
    {
        return SelectExercises(state).FirstOrDefault(e => e.Id == id);
    }

    public static IEnumerable<string> SelectTags(this RootState state)
    {
        return SelectExercises(state)
            .SelectMany(e => e.Tags)
            .Distinct();
    }

    public static ExercisesFilterViewModel SelectExercisesFilter(this RootState state)
    {
        return state.Exercises?.Filter ?? ExercisesFilterViewModel.Empty;
    }

    public static IEnumerable<ExerciseViewModel> SelectFilteredExercises(this RootState state)
    {
        var filter = state.SelectExercisesFilter();
        var allExercises = state.SelectExercises();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            allExercises = allExercises.Where(e => e.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.MuscleGroups.Any())
        {
            allExercises = allExercises.Where(e => e.Muscles.Any(m => filter.MuscleGroups.Contains(m.MuscleGroup)));
        }

        return allExercises;
    }
}
