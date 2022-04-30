using BlazorState.Redux.Utilities;

namespace WorkoutTracker.Data.Props;

public class ExerciseProfileSelectorProps
{
    public ExerciseProfile SelectedProfile { get; set; }

    public AsyncAction GenerateSchedule { get; set; }

    public AsyncAction<ExerciseProfile> SetProfile { get; set; }
}
