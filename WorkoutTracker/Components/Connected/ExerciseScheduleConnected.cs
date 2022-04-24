using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected
{
    public class ExerciseScheduleConnected : ConnectedComponent<ExerciseSchedule, RootState, ExerciseScheduleProps>
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapStateToProps(RootState state, ExerciseScheduleProps props)
        {
            props.Schedule = state.SelectSchedule();
            props.ExerciseCountLookup = state.SelectTodayExerciseCountLookup();
        }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseScheduleProps props)
        {
            props.Start = Callback<ScheduleViewModel>(item => Navigation.NavigateTo($"/trackexercise/{item.Id}"));
            props.Previous = Callback<ScheduleViewModel>(async model => await store.Dispatch<MoveToPreviousExerciseAction, ScheduleViewModel>(model));
            props.Next = Callback<ScheduleViewModel>(async model => await store.Dispatch<MoveToNextExerciseAction, ScheduleViewModel>(model));
        }

        protected override async Task Init(IStore<RootState> store)
        {
            if (store.State.SelectSchedule().Count() > 0)
            {
                return;
            }

            await store.Dispatch<FetchExercisesAction>();
            await store.Dispatch<BuildExerciseScheduleAction, ExerciseProfile>(store.State.SelectCurrentProfile());

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            store.Dispatch<FetchExerciseLogsAction, DateTime>(DateTime.Today);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            
        }
    }
}
