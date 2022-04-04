public class ExerciseProfile
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string[] MuscleGroups { get; set; }

    public bool IncludeCore { get; set; }

    public static ExerciseProfile UpperBody = new ExerciseProfile
    {
        Name = "Upper body",
        MuscleGroups = new[] { "Chest", "Back", "Shoulder", "Triceps", "Biceps" },
        IncludeCore = true
    };

    public static ExerciseProfile LowerBody = new ExerciseProfile
    {
        Name = "Lower body",
        MuscleGroups = new[] { "Glutes", "Quads", "Hamstrings", "Calves", "Adductors" },
        IncludeCore = true
    };
}
