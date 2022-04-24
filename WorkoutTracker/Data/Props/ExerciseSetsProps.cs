using BlazorState.Redux.Utilities;

namespace WorkoutTracker.Data.Props
{
    public class ExerciseSetsProps 
    {
        public LogEntryViewModel Log { get; set; }

        public PreviousLogRecordStats PreviousLog { get; set; }

        public bool PreviousLogLoading { get; set; }

        public AsyncAction<LogEntryViewModel> Save { get; set; }
    }
}
