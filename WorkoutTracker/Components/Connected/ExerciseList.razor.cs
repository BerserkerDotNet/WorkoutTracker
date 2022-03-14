using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Data.Actions;
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
        }

        protected override void MapStateToProps(RootState state, ExerciseListProps props)
        {
            props.List = SelectExercises(state).ToList();
        }

        protected override async Task Init(IStore<RootState> store)
        {
            if (!SelectExercises(store.State).Any())
            {
                await store.Dispatch<FetchExercisesAction>();
            }
        }
    }
}
