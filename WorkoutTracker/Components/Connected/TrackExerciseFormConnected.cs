using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class TrackExerciseFormConnected : SafeConnectedComponent<TrackExerciseForm, RootState, TrackExerciseFormProps>
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override void MapStateToPropsSafe(RootState state, TrackExerciseFormProps props)
    {
        var currentScheduleId = state.SelectCurentScheduleId();
        if (currentScheduleId is null)
        {
            return;
        }

        var nextSet = state.SelectNextExerciseFromSchedule(currentScheduleId.Value);
        props.NextExerciseId = nextSet.Id == currentScheduleId ? null : nextSet;

        var prevSet = state.SelectPreviousExerciseFromSchedule(currentScheduleId.Value);
        props.PreviousExerciseId = prevSet is null || prevSet.Id == currentScheduleId ? null : prevSet;

        var currentSchedule = state.SelectScheduleById(currentScheduleId.Value);
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
            var exercise = state.SelectExerciseById(currentExerciseId);
            props.Log = LogEntryViewModel.New(exercise);
            props.SetNumber = 1;
        }

        props.PreviousLog = state.SelectLastLogByExercise(currentExerciseId);
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

        props.AddNew = CallbackAsync(async () =>
        {
            var request = new InsertNewExerciseRequest(new MuscleGroupExerciseSelector("Back"), store.State.SelectSchedule(), store.State.SelectCurentScheduleId());
            await store.Dispatch<InsertNewExerciseAction, InsertNewExerciseRequest>(request);
        });

        props.Replace = CallbackAsync<ScheduleViewModel>(async model => await store.Dispatch<MoveToNextExerciseAction, ScheduleViewModel>(model));
        props.Swap = Callback<ScheduleViewModel>(model => store.Dispatch(new SwapExerciseSchedulesAction(model)));
    }

    protected override void OnParametersSet()
    {
        var currentSchedule = Store.State.SelectCurentScheduleId();
        if (currentSchedule is null)
        {
            Navigation.NavigateTo("/");
            return;
        }

        base.OnParametersSet();
    }
}
