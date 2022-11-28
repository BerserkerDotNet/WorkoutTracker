using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class ProgressIndicatorConnected : SafeConnectedComponent<ProgressIndicator, RootState, ProgressIndicatorProps>
{
    protected override void MapStateToPropsSafe(RootState state, ProgressIndicatorProps props)
    {
        var indicator = state.SelectCurrentProgressIndicator();
        props.IsVisible = indicator is object;
        props.Text = indicator?.Text;
    }

    protected override void MapDispatchToProps(IStore<RootState> store, ProgressIndicatorProps props)
    {
    }
}