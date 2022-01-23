using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Android;
using WorkoutTracker.MAUI.Data.Actions;
using WorkoutTracker.MAUI.ViewModels;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Components.Connected
{
    public class ExerciseScheduleConnected : ConnectedComponent<ExerciseSchedule, RootState, ExerciseScheduleProps>
    {
        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseScheduleProps props)
        {
            props.Start = EventCallback.Factory.Create<Guid>(this, id => Navigation.NavigateTo($"/editexerciselog/{Guid.Empty}/{id}"));
            props.OpenLog = EventCallback.Factory.Create(this, () => Navigation.NavigateTo($"/log"));
            props.Rebuild = EventCallback.Factory.Create(this, async () =>
            {
                await store.Dispatch<BuildExerciseScheduleAction, IEnumerable<ExerciseViewModel>>(store.State.Exercises.List.Values);
            });

            props.Replace = EventCallback.Factory.Create<ExerciseViewModel>(this, async e =>
            {
                var state = store.State.Exercises;
                await store.Dispatch<ReplaceExerciseAction, ReplaceExerciseRequest>( new ReplaceExerciseRequest(e, state.Schedule, state.List.Values));
            });
        }

        protected override void MapStateToProps(RootState state, ExerciseScheduleProps props)
        {
            props.Schedule = state?.Exercises?.Schedule ?? Enumerable.Empty<ExerciseViewModel>();
            if (state is object && state.Exercises is object && state.Exercises.Log is object)
            {
                props.ExerciseCount = state.Exercises.Log.GroupBy(g => g.Exercise.Id).ToDictionary(k => k.Key, v => v.Count());
            }
        }

        protected override async Task Init(IStore<RootState> store)
        {
            if (!ConfigurationService.IsConfigurationAvailable()) 
            {
                Navigation.NavigateTo("/configuration");
                return;
            }

            var state = store.State?.Exercises;
            if (state?.Schedule is object && state.Schedule.Any())
            {
                return;
            }

            if (state?.List is null)
            {
                await store.Dispatch<FetchExercisesAction>();
            }

            await store.Dispatch<BuildExerciseScheduleAction, IEnumerable<ExerciseViewModel>>(store.State.Exercises.List.Values);
        }
    }
}
