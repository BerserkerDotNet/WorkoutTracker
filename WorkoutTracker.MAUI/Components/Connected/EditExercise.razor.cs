using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using WorkoutTracker.MAUI.Data.Actions;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Components.Connected
{
    public class EditExerciseConnected : ConnectedComponent<EditExercise, RootState, EditExerciseProps>
    {
        private EditExerciseProps _props; // TODO: workaround

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
            props.Exercise = state.Exercises.List[ExerciseId];
            _props = props;
        }

        protected override void OnParametersSet()
        {
            MapStateToProps(Store.State, _props);
            this.StateHasChanged();
        }
    }
}
