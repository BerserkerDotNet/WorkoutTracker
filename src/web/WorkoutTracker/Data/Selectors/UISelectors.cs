namespace WorkoutTracker.Data.Selectors;

public static class UISelectors
{
    public static ProgressIndicatorViewModel SelectCurrentProgressIndicator(this RootState state)
    {
        var indicators = state.UI?.ProgressIndicators ?? new Stack<ProgressIndicatorViewModel>();
        if (indicators.Any())
        {
            return indicators.Peek();
        }

        return null;
    }
}
