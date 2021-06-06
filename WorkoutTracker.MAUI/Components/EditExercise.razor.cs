using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Actions;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Components
{
    public class EditExerciseConnected : ConnectedComponent<EditExercise, RootState, EditExerciseProps>
    {
        [Parameter]
        public Guid Id { get; set; }

        [Parameter]
        public Guid ExerciseId { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, EditExerciseProps props)
        {
            props.Save = EventCallback.Factory.Create<ExerciseLogEntry>(this, async e =>
            {
                await store.Dispatch<SaveExerciseLogEntryAction, ExerciseLogEntry>(e);
                Navigation.NavigateTo($"/");
            });

            props.Cancel = EventCallback.Factory.Create(this, () => Navigation.NavigateTo($"/"));
        }

        protected override void MapStateToProps(RootState state, EditExerciseProps props)
        {
            props.Exercise = state.Exercises.List[ExerciseId];
        }

        protected override async Task Init(IStore<RootState> store)
        {
            var state = store.State?.Exercises;
            if (state?.List is null)
            {
                await store.Dispatch<FetchExercisesAction>();
            }
        }
    }
}
