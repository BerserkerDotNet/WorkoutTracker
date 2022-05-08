using Microsoft.AspNetCore.Components;
using WorkoutTracker.Components.Presentational;
using WorkoutTracker.Data.Actions;
using WorkoutTracker.Data.Selectors;

namespace WorkoutTracker.Components.Connected;

public class ExerciseSetDetailsConnected : ConnectedComponent<ExerciseSetDetails, RootState, ExerciseHistoryDetailsProps>
{
    [Inject]
    public NavigationManager Navigation { get; set; }

    [Parameter]
    [EditorRequired]
    public Guid RecordId { get; set; }

    [Parameter]
    [EditorRequired]
    public DateTime Date { get; set; }

    protected override void MapStateToProps(RootState state, ExerciseHistoryDetailsProps props)
    {
        props.LogRecord = state.SelectExerciseLog(Date.ToDateOnly(), RecordId); // TODO: Handle null and also load exercise initally
    }

    protected override void MapDispatchToProps(IStore<RootState> store, ExerciseHistoryDetailsProps props)
    {
        props.Save = CallbackAsync<LogEntryViewModel>(async e => await store.Dispatch<SaveExerciseLogEntryAction, LogEntryViewModel>(e));
        props.Back = Callback(() => Navigation.NavigateTo("/log"));
    }
}
