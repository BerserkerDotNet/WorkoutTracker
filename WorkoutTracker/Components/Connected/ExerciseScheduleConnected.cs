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
            props.AllExercises = state.SelectExercises();
            props.Schedule = state.SelectSchedule();
            props.CurrentScheduleId = state.SelectCurentScheduleId();
            props.TodayLogByExercise = state.SelectTodayExerciseLogLookup();
            props.PreviousSessionLog = state.SelectLastLogByExercise(props.Schedule);
        }

        protected override void MapDispatchToProps(IStore<RootState> store, ExerciseScheduleProps props)
        {
            props.AddExercise = CallbackAsync<IExerciseSelector>(async (selector) =>
            {
                var request = new InsertNewExerciseRequest(selector, store.State.SelectSchedule(), store.State.SelectCurentScheduleId());
                await store.Dispatch<InsertNewExerciseAction, InsertNewExerciseRequest>(request);
            });
            props.Save = CallbackAsync<LogEntryViewModel>(async e =>
            {
                await store.Dispatch<SaveExerciseLogEntryAction, LogEntryViewModel>(e);
            });
            props.RemoveExercise = CallbackAsync<Guid>(async id => await store.Dispatch<RemoveExerciseFromSchedule, RemoveExerciseFromScheduleRequest>(new RemoveExerciseFromScheduleRequest(store.State.SelectSchedule(), id)));
            props.SetScheduleTargetSets = (id, targetSets) => Callback(() => store.Dispatch(new SetScheduleTargetSets(id, targetSets)))();

            props.ReplaceExercise = (id, exercise) => Callback(() => store.Dispatch(new ReplaceScheduleExercise(id, exercise)))();
        }

        protected override async Task Init(IStore<RootState> store)
        {
            var state = store.State;
            if (state.ExerciseSchedule is null)
            {
                await store.Dispatch<FetchExercisesAction>();
                await store.Dispatch<BuildExerciseScheduleAction, ExerciseProfile>(state.SelectCurrentProfile());
            }

            var isSummariesLoaded = state.SelectSummaries().Any();
            if (!isSummariesLoaded)
            {
                await store.Dispatch<FetchWorkoutStatsAction, WorkoutStatsRequest>(new WorkoutStatsRequest(DateTime.Today.AddMonths(-6), DateTime.Today.AddDays(1)));
            }

            var history = store.State.SelectHistory();
            if (history.ContainsKey(DateTime.Today.ToDateOnly()))
            {
                return;
            }

            await store.Dispatch<FetchExerciseLogsAction, DateTime>(DateTime.Today);
        }
    }
}
