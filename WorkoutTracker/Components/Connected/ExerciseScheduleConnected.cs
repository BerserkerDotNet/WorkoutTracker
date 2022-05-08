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
        }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseScheduleProps props)
        {
            props.Start = Callback<IEnumerable<ScheduleViewModel>>(schedule => 
            {
                store.Dispatch(new ReceiveExerciseScheduleAction(schedule.ToArray()));
                var first = schedule.First();
                Navigation.NavigateTo($"/trackexercise/{first.Id}");
            });
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
        }
    }
}
