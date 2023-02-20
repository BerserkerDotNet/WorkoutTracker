namespace WorkoutTracker.Data.Selectors;

public static class UserPreferencesSelectors
{
    public static bool SelectShowWeightInKG(this RootState state)
    {
        return state.Preferences?.ShowWeightInKG ?? false;
    }
}