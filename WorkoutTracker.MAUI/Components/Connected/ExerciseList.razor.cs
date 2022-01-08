using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Actions;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Components.Connected
{
    public class ExerciseListConnected : ConnectedComponent<ExerciseList, RootState, ExerciseListProps>
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseListProps props)
        {
            props.Edit = EventCallback.Factory.Create<Exercise>(this, e =>
            {
                Navigation.NavigateTo($"/editexercise/{e.Id}");
            });
        }

        protected override void MapStateToProps(RootState state, ExerciseListProps props)
        {
            props.List = state?.Exercises?.List?.Values ?? Enumerable.Empty<Exercise>();
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
