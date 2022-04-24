using BlazorState.Redux.Utilities;

namespace WorkoutTracker.Data.Props;

public class TrackExerciseFormProps 
{
    public ScheduleViewModel NextExerciseId { get; set; }

    public ScheduleViewModel PreviousExerciseId { get; set; }

    public LogEntryViewModel Log { get; set; }

    public int SetNumber { get; set; }

    public AsyncAction<LogEntryViewModel> Save { get; set; }

    public Action<ScheduleViewModel> Next { get; set; }

    public Action<ScheduleViewModel> Previous { get; set; }

    public Action Cancel { get; set; }
}
