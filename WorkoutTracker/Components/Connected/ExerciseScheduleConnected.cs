using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected
{
    public class ExerciseScheduleConnected : SafeConnectedComponent<ExerciseSchedule, RootState, ExerciseScheduleProps>
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override void MapStateToPropsSafe(RootState state, ExerciseScheduleProps props)
        {
            props.Schedule = state.SelectSchedule();
            props.CurrentScheduleId = state.SelectCurentScheduleId();
        }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseScheduleProps props)
        {
            props.Start = Callback<IEnumerable<ScheduleViewModel>>(schedule =>
            {
                var first = schedule.First();
                store.Dispatch(new SetCurrentSchedule(first.Id));
                Navigation.NavigateTo($"/trackexercise/{first.Id}");
            });
            props.Resume = Callback<Guid>(scheduleId =>
            {
                Navigation.NavigateTo($"/trackexercise/{scheduleId}");
            });
            props.Previous = Callback<ScheduleViewModel>(async model => await store.Dispatch<MoveToPreviousExerciseAction, ScheduleViewModel>(model));
            props.Next = Callback<ScheduleViewModel>(async model => await store.Dispatch<MoveToNextExerciseAction, ScheduleViewModel>(model));
            props.MoveUp = Callback<ScheduleViewModel>(model => store.Dispatch(new MoveExerciseUpAction(model)));
            props.MoveDown = Callback<ScheduleViewModel>(model => store.Dispatch(new MoveExerciseDownAction(model)));
        }

        protected override async Task Init(IStore<RootState> store)
        {
            var state = store.State;
            if (state.ExerciseSchedule is null)
            {
                await store.Dispatch<FetchExercisesAction>();
                await store.Dispatch<BuildExerciseScheduleAction, ExerciseProfile>(state.SelectCurrentProfile());
            }
            await store.Dispatch<FetchWorkoutStatsAction, WorkoutStatsRequest>(new WorkoutStatsRequest(DateTime.Today.AddMonths(-6), DateTime.Today.AddDays(1)));
        }
    }
}
