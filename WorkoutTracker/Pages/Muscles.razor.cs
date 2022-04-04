using Microsoft.AspNetCore.Components;
using WorkoutTracker.Data.Actions;
using static WorkoutTracker.Data.Selectors.MuscleSelectors;

namespace WorkoutTracker.Pages;

public partial class Muscles
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override void MapStateToProps(RootState state, MusclesListProps props)
    {
        props.Muscles = SelectMuscles(state);
    }

    protected override void MapDispatchToProps(IStore<RootState> store, MusclesListProps props)
    {
        props.Edit = CreateCallback<Guid>(id => Navigation.NavigateTo($"/editmuscle/{id}"));
    }

    protected override async Task Init(IStore<RootState> store)
    {
        if (!SelectMuscles(store.State).Any())
        {
            await store.Dispatch<FetchMusclesAction>();
        }
    }
}
