using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;
using WorkoutTracker.Models;

namespace WorkoutTracker.Components.Connected;

public class TrackExerciseFormConnected : ConnectedComponent<TrackExerciseForm, RootState, TrackExerciseFormProps>
{
    [Parameter]
    public Guid CurrentScheduleId { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override void MapStateToProps(RootState state, TrackExerciseFormProps props)
    {
        var nextSet = state.SelectNextExerciseFromSchedule(CurrentScheduleId);
        props.NextExerciseId = nextSet.Id == CurrentScheduleId ? null : nextSet;

        var prevSet = state.SelectPreviousExerciseFromSchedule(CurrentScheduleId);
        props.PreviousExerciseId = prevSet is null || prevSet.Id == CurrentScheduleId ? null : prevSet;

        var currentSchedule = state.SelectScheduleById(CurrentScheduleId);
        var currentExerciseId = currentSchedule.CurrentExercise.Id;

        var log = state.SelectTodayExerciseById(currentExerciseId);
        if (log is object)
        {
            props.Log = log;
            props.SetNumber = log.Sets.Count() + 1;
        }
        else
        {
            props.Log = new LogEntryViewModel
            {
                Id = Guid.NewGuid(),
                Exercise = state.SelectExerciseById(currentExerciseId),
                Sets = Enumerable.Empty<Set>(),
                Date = DateTime.UtcNow
            };
            props.SetNumber = 1;
        }
    }

    protected override void MapDispatchToProps(IStore<RootState> store, TrackExerciseFormProps props)
    {
        props.Save = CallbackAsync<LogEntryViewModel>(async e =>
        {
            await store.Dispatch<SaveExerciseLogEntryAction, LogEntryViewModel>(e);
        });

        props.Cancel = Callback(() => Navigation.NavigateTo($"/"));

        props.Next = Callback<ScheduleViewModel>(item =>
        {
            Navigation.NavigateTo($"/trackexercise/{item.Id}");
        });

        props.Previous = Callback<ScheduleViewModel>(item =>
        {
            Navigation.NavigateTo($"/trackexercise/{item.Id}");
        });
    }
}
