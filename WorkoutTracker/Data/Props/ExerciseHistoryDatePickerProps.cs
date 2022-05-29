using BlazorState.Redux.Utilities;

namespace WorkoutTracker.Data.Props;

public class ExercisesHistoryProps 
{
    public IEnumerable<LogEntryViewModel> Records { get; set; }

    public DateOnly SelectedDate { get; set; }

    public bool IsLoadingRecods { get; set; }

    public bool ShowWeightInKG { get; set; }

    public Action<LogEntryViewModel> ViewDetails { get; set; }

    public AsyncAction<DateOnly> SelectedDateChanged { get; set; }

    public AsyncAction<Guid> Delete { get; set; }
}