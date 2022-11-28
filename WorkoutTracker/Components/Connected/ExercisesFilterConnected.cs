using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class ExercisesFilterConnected : SafeConnectedComponent<ExercisesFilter, RootState, ExercisesFilterProps>
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override void MapStateToPropsSafe(RootState state, ExercisesFilterProps props)
    {
        props.Filter = state.SelectExercisesFilter();
        props.MuscleGroups = state.SelectMuscleGroups().ToArray();
    }

    protected override void MapDispatchToProps(IStore<RootState> store, ExercisesFilterProps props)
    {
        props.Add = Callback(() => Navigation.NavigateTo($"/editexercise/{Guid.NewGuid()}"));
        props.ApplyFilter = Callback<ExercisesFilterViewModel>(filter => store.Dispatch(new ExercisesFilterChanged(filter)));
    }

    protected override async Task Init(IStore<RootState> store)
    {
        if (!store.State.SelectMuscles().Any())
        {
            await store.Dispatch<FetchMusclesAction>();
        }
    }
}
