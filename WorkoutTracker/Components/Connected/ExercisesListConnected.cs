using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class ExercisesListConnected : SafeConnectedComponent<ExercisesList, RootState, ExercisesListProps>
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override void MapStateToPropsSafe(RootState state, ExercisesListProps props)
    {
        props.List = state.SelectFilteredExercises().ToList();
    }

    protected override void MapDispatchToProps(IStore<RootState> store, ExercisesListProps props)
    {
        props.Edit = Callback<ExerciseViewModel>(e => Navigation.NavigateTo($"/editexercise/{e.Id}"));
        props.Delete = CallbackAsync<Guid>(async id => await store.Dispatch<DeleteExerciseAction, Guid>(id));
    }

    protected override async Task Init(IStore<RootState> store)
    {
        if (!store.State.SelectExercises().Any())
        {
            await store.Dispatch<FetchExercisesAction>();
        }
    }
}
