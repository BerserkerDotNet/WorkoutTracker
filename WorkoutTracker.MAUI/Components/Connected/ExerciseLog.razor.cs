using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Actions;
using static WorkoutTracker.MAUI.Data.Selectors.ExerciseHistorySelectors;

namespace WorkoutTracker.MAUI.Components.Connected
{
    public class ExerciseLogConnected : ConnectedComponent<ExerciseLog, RootState, ExercisesLogProps>
    {
        protected override void MapDispatchToProps(IStore<RootState> store, ExercisesLogProps props)
        {
            props.Load = EventCallback.Factory.Create<DateTime>(this, async date =>
            {
                await store.Dispatch<FetchExerciseLogsAction, DateTime>(date);
            });

            props.Delete = EventCallback.Factory.Create<Guid>(this, async id =>
            {
                await store.Dispatch<DeleteExerciseLogEntryAction, Guid>(id);
            });
        }

        protected override void MapStateToProps(RootState state, ExercisesLogProps props)
        {
            props.Log = SelectHistory(state);
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
