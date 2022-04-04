namespace WorkoutTracker.Data.Props;

public class EditMuiscleProps
{
    public MuscleViewModel Muscle { get; set; }

    public Action<MuscleViewModel> Save { get; set; }

    public Action Cancel { get; set; }
}

public static class MuscleGroups 
{
    public static string[] Groups = new[]
    {
        "Arm", "Chest", "Back", "Shoulder", "Triceps", "Biceps", "Core", "Glutes", "Quads", "Hamstrings", "Calves", "Adductors"
    }.OrderBy(g => g).ToArray();
}
