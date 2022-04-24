using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;
using WorkoutTracker.Models;

namespace WorkoutTracker.Components.Connected;

public class ExerciseSetsConnected : ConnectedComponent<ExerciseSets, RootState, ExerciseSetsProps>
{
    [Parameter]
    public Guid CurrentScheduleId { get; set; }

    protected override void MapStateToProps(RootState state, ExerciseSetsProps props)
    {
        var currentSchedule = state.SelectScheduleById(CurrentScheduleId);
        var currentExerciseId = currentSchedule.CurrentExercise.Id;

        var log = state.SelectTodayExerciseById(currentExerciseId);

        props.Log = log ?? new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Exercise = state.SelectExerciseById(currentExerciseId),
            Sets = Enumerable.Empty<Set>(),
            Date = DateTime.UtcNow
        };

        props.PreviousLog = state.SelectLastLogByExercise(currentExerciseId);
        props.PreviousLogLoading = !state.IsLastLogByExerciseLoaded(currentExerciseId);
    }

    protected override void MapDispatchToProps(IStore<RootState> store, ExerciseSetsProps props)
    {
        props.Save = CallbackAsync<LogEntryViewModel>(async e =>
        {
            await store.Dispatch<SaveExerciseLogEntryAction, LogEntryViewModel>(e);
        });
    }

    protected override async Task OnParametersSetAsync()
    {
        var currentSchedule = Store.State.SelectScheduleById(CurrentScheduleId);
        var currentExerciseId = currentSchedule.CurrentExercise.Id;

        if (!Store.State.IsLastLogByExerciseLoaded(currentExerciseId))
        {
            await Store.Dispatch<FetchLastWorkoutByExerciseAction, Guid>(currentExerciseId);
        }

        await base.OnParametersSetAsync();
    }
}
