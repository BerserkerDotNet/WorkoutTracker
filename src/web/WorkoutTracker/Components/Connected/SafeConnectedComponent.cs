using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;

namespace WorkoutTracker.Components.Connected;

public abstract class SafeConnectedComponent<TComponent, TState, TProps> : ConnectedComponent<TComponent, TState, TProps>
    where TComponent : ComponentBase where TProps : new()
{
    [Inject]
    public ApplicationContext<ComponentBase> Context { get; set; }

    protected sealed override void MapStateToProps(TState state, TProps props)
    {
        try
        {
            MapStateToPropsSafe(state, props);
        }
        catch (Exception ex)
        {
            Context.LogError(ex, "Failed to map props for '{Component}'", GetType().Name);
            Context.ShowError(ex.Message);
            throw;
        }
    }

    protected abstract void MapStateToPropsSafe(TState state, TProps props);
}