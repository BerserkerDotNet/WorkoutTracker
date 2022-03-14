using Microsoft.AspNetCore.Components;

namespace WorkoutTracker.Data.Props
{
    public class LogRecordDetailsProps
	{
		public LogEntryViewModel LogRecord { get; set; }

		public EventCallback<LogEntryViewModel> OnSave { get; set; }

		public EventCallback OnBack { get; set; }
	}
}
