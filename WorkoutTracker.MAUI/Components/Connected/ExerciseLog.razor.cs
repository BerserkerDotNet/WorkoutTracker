using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Actions;

namespace WorkoutTracker.MAUI.Components.Connected
{
    public class ExerciseLogConnected : ConnectedComponent<ExerciseLog, RootState, ExercisesLogProps>
    {
        protected override void MapDispatchToProps(IStore<RootState> store, ExercisesLogProps props)
        {
            props.Delete = EventCallback.Factory.Create<Guid>(this, async id =>
            {
                await store.Dispatch<DeleteExerciseLogEntryAction, Guid>(id);
            });
        }

        protected override void MapStateToProps(RootState state, ExercisesLogProps props)
        {
            props.Log = new List<LogEntryViewModel>(state?.Exercises?.Log ?? Enumerable.Empty<LogEntryViewModel>());
        }

        protected override async Task Init(IStore<RootState> store)
        {
            var state = store.State?.Exercises;
            if (state?.Log is object)
            {
                return;
            }

            await store.Dispatch<FetchExerciseLogsAction>();
        }
    }
}
