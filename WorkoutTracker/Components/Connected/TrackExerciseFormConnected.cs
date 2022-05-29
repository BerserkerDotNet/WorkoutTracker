using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;
using WorkoutTracker.Models;

namespace WorkoutTracker.Components.Connected;

public class TrackExerciseFormConnected : SafeConnectedComponent<TrackExerciseForm, RootState, TrackExerciseFormProps>
{
    [Parameter]
    public Guid CurrentScheduleId { get; set; }

    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override void MapStateToPropsSafe(RootState state, TrackExerciseFormProps props)
    {
        var nextSet = state.SelectNextExerciseFromSchedule(CurrentScheduleId);
        props.NextExerciseId = nextSet.Id == CurrentScheduleId ? null : nextSet;

        var prevSet = state.SelectPreviousExerciseFromSchedule(CurrentScheduleId);
        props.PreviousExerciseId = prevSet is null || prevSet.Id == CurrentScheduleId ? null : prevSet;

        var currentSchedule = state.SelectScheduleById(CurrentScheduleId);
        props.CurrentSchedule = currentSchedule;

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

        props.PreviousLog = state.SelectLastLogByExercise(currentExerciseId);
        props.PreviousLogLoading = !state.IsLastLogByExerciseLoaded(currentExerciseId);
    }

    protected override void MapDispatchToProps(IStore<RootState> store, TrackExerciseFormProps props)
    {
        props.Save = CallbackAsync<LogEntryViewModel>(async e =>
        {
            await store.Dispatch<SaveExerciseLogEntryAction, LogEntryViewModel>(e);
            if (e.Sets.Count() == props.CurrentSchedule.TargetSets && props.NextExerciseId is object) 
            {
                Navigation.NavigateTo($"/trackexercise/{props.NextExerciseId.Id}");
            }
        });

        props.Cancel = Callback(() => Navigation.NavigateTo($"/"));

        props.Next = Callback<ScheduleViewModel>(item =>
        {
            store.Dispatch(new SetCurrentSchedule(item.Id));
            Navigation.NavigateTo($"/trackexercise/{item.Id}");
        });

        props.Previous = Callback<ScheduleViewModel>(item =>
        {
            store.Dispatch(new SetCurrentSchedule(item.Id));
            Navigation.NavigateTo($"/trackexercise/{item.Id}");
        });

        props.Replace = CallbackAsync<ScheduleViewModel>(async model => await store.Dispatch<MoveToNextExerciseAction, ScheduleViewModel>(model));
        props.Swap = Callback<ScheduleViewModel>(model => store.Dispatch(new SwapExerciseSchedulesAction(model)));
    }

    protected override async Task OnParametersSetAsync()
    {
        var currentSchedule = Store.State.SelectScheduleById(CurrentScheduleId);
        if (currentSchedule is null) 
        {
            Navigation.NavigateTo("/");
            return;
        }

        var currentExerciseId = currentSchedule.CurrentExercise.Id;
        if (!Store.State.IsLastLogByExerciseLoading(currentExerciseId))
        {
            await Store.Dispatch<FetchLastWorkoutByExerciseAction, Guid>(currentExerciseId);
        }

        base.OnParametersSet();
    }
}
