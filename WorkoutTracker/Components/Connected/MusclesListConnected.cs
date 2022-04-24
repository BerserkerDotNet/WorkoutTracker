using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class MusclesListConnected : ConnectedComponent<MusclesList, RootState, MusclesListProps>
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override void MapStateToProps(RootState state, MusclesListProps props)
    {
        props.Muscles = state.SelectMuscles();
    }

    protected override void MapDispatchToProps(IStore<RootState> store, MusclesListProps props)
    {
        props.Edit = Callback<Guid>(id => Navigation.NavigateTo($"/editmuscle/{id}"));
    }

    protected override async Task Init(IStore<RootState> store)
    {
        if (!store.State.SelectMuscles().Any())
        {
            await store.Dispatch<FetchMusclesAction>();
        }
    }
}
