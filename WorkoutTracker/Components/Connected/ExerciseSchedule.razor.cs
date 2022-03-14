using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTracker.Data.Actions;
using static WorkoutTracker.Data.Selectors.ScheduleSelectors;
using static WorkoutTracker.Data.Selectors.ExerciseHistorySelectors;
using static WorkoutTracker.Data.Selectors.ExerciseSelectors;

namespace WorkoutTracker.Components.Connected
{
    public class ExerciseScheduleConnected : ConnectedComponent<ExerciseSchedule, RootState, ExerciseScheduleProps>
    {
        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseScheduleProps props)
        {
            props.Start = EventCallback.Factory.Create<ExerciseWithCategoryViewModel>(this, item => Navigation.NavigateTo($"/editexerciselog/{item.Category}/{item.Exercise.Id}"));
            props.OpenLog = EventCallback.Factory.Create(this, () => Navigation.NavigateTo($"/log"));
            props.Rebuild = EventCallback.Factory.Create<ExerciseProfile>(this, async profile =>
            {
                var exercises = SelectExercises(store.State);
                // TODO: Should this be here?
                if (profile == ExerciseProfile.UpperBody)
                {
                    await store.Dispatch<BuildUpperBodyExerciseScheduleAction, IEnumerable<ExerciseViewModel>>(exercises);
                }
                else 
                {
                    await store.Dispatch<BuildLowerBodyExerciseScheduleAction, IEnumerable<ExerciseViewModel>>(exercises);
                }
            });

            props.Previous = EventCallback.Factory.Create<string>(this, async category =>
            {
                await store.Dispatch<MoveToPreviousExerciseAction, ScheduleViewModel>(SelectScheduleByCategory(store.State, category));
            });

            props.Next = EventCallback.Factory.Create<string>(this, async category =>
            {
                await store.Dispatch<MoveToNextExerciseAction, ScheduleViewModel>(SelectScheduleByCategory(store.State, category));
            });
        }

        protected override void MapStateToProps(RootState state, ExerciseScheduleProps props)
        {
            props.Schedule = SelectCurrentExercisesFromSchedule(state);
            props.ExerciseCountLookup = SelectTodayExerciseCountLookup(state);
        }

        protected override async Task Init(IStore<RootState> store)
        {
            /*if (!ConfigurationService.IsConfigurationAvailable()) 
            {
                Navigation.NavigateTo("/configuration");
                return;
            }*/

            if (SelectSchedule(store.State).Count > 0)
            {
                return;
            }

            await store.Dispatch<FetchExercisesAction>();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			store.Dispatch<FetchExerciseLogsAction, DateTime>(DateTime.Today);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
			await store.Dispatch<BuildUpperBodyExerciseScheduleAction, IEnumerable<ExerciseViewModel>>(SelectExercises(store.State));
        }
    }
}
