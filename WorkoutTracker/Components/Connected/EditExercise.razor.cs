using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using WorkoutTracker.Data.Actions;
using static WorkoutTracker.Data.Selectors.ExerciseSelectors;

namespace WorkoutTracker.Components.Connected
{
    public class EditExerciseConnected : ConnectedComponent<EditExercise, RootState, EditExerciseProps>
    {
        [Parameter]
        public Guid ExerciseId { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, EditExerciseProps props)
        {
            props.Save = EventCallback.Factory.Create<ExerciseViewModel>(this, async exercise =>
            {
                await store.Dispatch<SaveExerciseAction, ExerciseViewModel>(exercise);
                Navigation.NavigateTo($"/library");
            });

            props.Cancel = EventCallback.Factory.Create(this, () => Navigation.NavigateTo($"/library"));
        }

        protected override void MapStateToProps(RootState state, EditExerciseProps props)
        {
            props.Exercise = SelectExerciseById(state, ExerciseId);
        }
    }
}
