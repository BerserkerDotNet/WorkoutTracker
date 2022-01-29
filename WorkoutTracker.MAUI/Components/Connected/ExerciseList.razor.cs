using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Actions;

namespace WorkoutTracker.MAUI.Components.Connected
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
        }

        protected override void MapStateToProps(RootState state, ExerciseListProps props)
        {
            props.List = new List<ExerciseViewModel>(state?.Exercises?.List?.Values ?? Enumerable.Empty<ExerciseViewModel>());
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
