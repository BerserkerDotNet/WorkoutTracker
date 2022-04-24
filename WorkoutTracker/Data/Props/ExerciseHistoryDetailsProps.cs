using BlazorState.Redux.Utilities;

namespace WorkoutTracker.Data.Props;

public class ExerciseHistoryDetailsProps
{
	public LogEntryViewModel LogRecord { get; set; }

	public AsyncAction<LogEntryViewModel> Save { get; set; }

	public Action Back { get; set; }
}
