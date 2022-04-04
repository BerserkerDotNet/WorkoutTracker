using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using WorkoutTracker.Data.Actions;
using static WorkoutTracker.Data.Selectors.MuscleSelectors;
using static WorkoutTracker.Data.Selectors.ExerciseSelectors;

namespace WorkoutTracker.Components.Connected
{
    public class ExerciseListConnected : ConnectedComponent<ExerciseList, RootState, ExerciseListProps>
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseListProps props)
        {
            props.Edit = EventCallback.Factory.Create<ExerciseViewModel>(this, e =>
            {
                Navigation.NavigateTo($"/editexercise/{e.Id}");
            });

            props.Delete = EventCallback.Factory.Create<Guid>(this, async id =>
            {
                await store.Dispatch<DeleteExerciseAction, Guid>(id);
            });

            props.Add = EventCallback.Factory.Create(this, e =>
            {
                Navigation.NavigateTo($"/editexercise/{Guid.NewGuid()}");
            });
        }

        protected override void MapStateToProps(RootState state, ExerciseListProps props)
        {
            props.List = SelectExercises(state).ToList();
            props.MuscleGroups = SelectMuscleGroups(state).ToArray();
        }

        protected override async Task Init(IStore<RootState> store)
        {
            if (!SelectExercises(store.State).Any())
            {
                await store.Dispatch<FetchExercisesAction>();
                await store.Dispatch<FetchMusclesAction>();
            }
        }
    }
}
