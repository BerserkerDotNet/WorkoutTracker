namespace WorkoutTracker.Data.Selectors;

public static class MuscleSelectors
{
    public static IEnumerable<MuscleViewModel> SelectMuscles(this RootState state)
    {
        return state.Exercises?.Muscles ?? Enumerable.Empty<MuscleViewModel>();
    }

    public static MuscleViewModel SelectMuscleById(this RootState state, Guid id)
    {
        return SelectMuscles(state).FirstOrDefault(e => e.Id == id);
    }

    public static IEnumerable<string> SelectMuscleGroups(this RootState state)
    {
        return SelectMuscles(state).Select(m => m.MuscleGroup).OrderBy(g => g).Distinct();
    }

    public static Dictionary<Guid, MuscleViewModel> SelectMusclesLookup(RootState state)
    {
        return SelectMuscles(state).ToDictionary(k => k.Id, v => v);
    }
}