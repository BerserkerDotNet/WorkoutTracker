using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class ExercisesHistoryConnected : SafeConnectedComponent<ExercisesHistory, RootState, ExercisesHistoryProps>
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    protected override void MapStateToPropsSafe(RootState state, ExercisesHistoryProps props)
    {
        var history = state.SelectHistory();
        props.SelectedDate = state.SelectDate();
        props.Records = state.SelectExercisesByDate(props.SelectedDate);
        props.IsLoadingRecods = !history.ContainsKey(props.SelectedDate);
        props.ShowWeightInKG = state.SelectShowWeightInKG();
    }

    protected override void MapDispatchToProps(IStore<RootState> store, ExercisesHistoryProps props)
    {
        props.Delete = CallbackAsync<Guid>(async id => await store.Dispatch<DeleteExerciseLogEntryAction, Guid>(id));
        props.SelectedDateChanged = CallbackAsync<DateOnly>(async date =>
        {
            store.Dispatch(new SetSelectedHistoryDate(date));
            await store.Dispatch<FetchExerciseLogsAction, DateTime>(date.ToDateTime());
        });
        props.ViewDetails = Callback<LogEntryViewModel>(model => Navigation.NavigateTo($"/logrecorddetails/{model.Date.ToString("yyyy-MM-dd")}/{model.Id}"));
    }

    protected override async Task Init(IStore<RootState> store)
    {
        var history = store.State.SelectHistory();
        var dateToLoad = store.State.SelectDate();
        if (history.ContainsKey(dateToLoad))
        {
            return;
        }

        await store.Dispatch<FetchExerciseLogsAction, DateTime>(DateTime.Today);
    }
}