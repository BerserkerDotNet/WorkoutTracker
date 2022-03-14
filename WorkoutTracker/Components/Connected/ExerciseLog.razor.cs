using BlazorState.Redux.Blazor;
using System.Threading.Tasks;
using WorkoutTracker.Data.Actions;
using static WorkoutTracker.Data.Selectors.ExerciseHistorySelectors;

namespace WorkoutTracker.Components.Connected
{
    public class ExerciseLogConnected : ConnectedComponent<ExerciseLog, RootState, ExercisesLogProps>
    {
        protected override void MapDispatchToProps(IStore<RootState> store, ExercisesLogProps props)
        {
            props.Load = CreateCallback<DateTime>(async date =>
            {
                await store.Dispatch<FetchExerciseLogsAction, DateTime>(date);
            });

            props.Save = CreateCallback<LogEntryViewModel>(async e =>
            {
                await store.Dispatch<SaveExerciseLogEntryAction, LogEntryViewModel>(e);
            });

            props.Delete = CreateCallback<Guid>(async id =>
            {
                await store.Dispatch<DeleteExerciseLogEntryAction, Guid>(id);
            });
        }

        protected override void MapStateToProps(RootState state, ExercisesLogProps props)
        {
            props.Log = SelectHistory(state);
            props.SelectedDate = SelectDate(state);
        }

        protected override async Task Init(IStore<RootState> store)
        {
            var history = SelectHistory(store.State);
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (history.ContainsKey(today))
            {
                return;
            }

            await store.Dispatch<FetchExerciseLogsAction, DateTime>(DateTime.Today);
        }
    }
}
