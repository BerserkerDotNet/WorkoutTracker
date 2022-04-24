namespace WorkoutTracker.Data.Props;

public class ExerciseProfileSelectorProps
{
    public ExerciseProfile SelectedProfile { get; set; }

    public Action<ExerciseProfile> OnProfileChanged { get; set; }
}
