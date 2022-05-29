namespace WorkoutTracker.Data.Reducers;

public record UserPreferencesState(bool ShowWeightInKG);

public class UserPreferencesReducer : IReducer<UserPreferencesState>
{
    public UserPreferencesState Reduce(UserPreferencesState state, IAction action)
    {
        state = state ?? new UserPreferencesState(false);

        switch (action)
        {
            default:
                return state;
        }
    }
}