using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Actions;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Components
{
    public class ExerciseScheduleConnected : ConnectedComponent<ExerciseSchedule, RootState, ExerciseScheduleProps>
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseScheduleProps props)
        {
            props.Start = EventCallback.Factory.Create<Guid>(this, id => Navigation.NavigateTo($"/editexerciselog/{Guid.Empty}/{id}"));
            props.OpenLog = EventCallback.Factory.Create(this, () => Navigation.NavigateTo($"/log"));
            props.Rebuild = EventCallback.Factory.Create(this, async () =>
            {
                await store.Dispatch<BuildExerciseScheduleAction, IEnumerable<Exercise>>(store.State.Exercises.List.Values);
            });

            props.Replace = EventCallback.Factory.Create<Exercise>(this, async e =>
            {
                var state = store.State.Exercises;
                await store.Dispatch<ReplaceExerciseAction, ReplaceExerciseRequest>( new ReplaceExerciseRequest(e, state.Schedule, state.List.Values));
            });
        }

        protected override void MapStateToProps(RootState state, ExerciseScheduleProps props)
        {
            props.Schedule = state?.Exercises?.Schedule ?? Enumerable.Empty<Exercise>();
            if (state is object && state.Exercises is object && state.Exercises.Log is object)
            {
                props.ExerciseCount = state.Exercises.Log.GroupBy(g => g.ExerciseId).ToDictionary(k => k.Key, v => v.Count());
            }
        }

        protected override async Task Init(IStore<RootState> store)
        {
            var state = store.State?.Exercises;
            if (state?.Schedule is object)
            {
                return;
            }

            if (state?.List is null)
            {
                await store.Dispatch<FetchExercisesAction>();
            }

            await store.Dispatch<BuildExerciseScheduleAction, IEnumerable<Exercise>>(store.State.Exercises.List.Values);
        }
    }
}
