using BlazorState.Redux.Utilities;
using WorkoutTracker.Data.Actions;

namespace WorkoutTracker.Data.Props;

public class EditMuscleFormProps
{
    public MuscleViewModel Muscle { get; set; }

    public AsyncAction<SaveMuscleModel> Save { get; set; }

    public Action Cancel { get; set; }
}

public static class MuscleGroups 
{
    public static string[] Groups = new[]
    {
        "Arm", "Chest", "Back", "Shoulder", "Triceps", "Biceps", "Core", "Glutes", "Quads", "Hamstrings", "Calves", "Adductors"
    }.OrderBy(g => g).ToArray();
}
