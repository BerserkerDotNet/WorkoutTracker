public class ExerciseProfile
{
    public string Name { get; set; }

    public string Description { get; set; }

    public IExerciseFilter[] ExerciseFilters { get; set; }

    public bool IncludeCore { get; set; }

    public int ShuffleStartIndex { get; set; } = -1;

    public static ExerciseProfile UpperBody = new ExerciseProfile
    {
        Name = "Upper body",
        ExerciseFilters = new MuscleGroupExerciseFilter[] { "Chest", "Back", "Shoulder", "Triceps", "Biceps" },
        IncludeCore = true
    };

    public static ExerciseProfile LowerBody = new ExerciseProfile
    {
        Name = "Lower body",
        ExerciseFilters = new MuscleGroupExerciseFilter[] { "Glutes", "Quads", "Quads", "Hamstrings", "Calves", "Adductors" },
        IncludeCore = false
    };

    public static ExerciseProfile ShouldersDay = new ExerciseProfile
    {
        Name = "Shoulders day",
        ExerciseFilters = new IExerciseFilter[]
        {
            new MuscleExerciseFilter(Guid.Parse("9024dbc7-defb-4aa3-8b0d-29ce21d1ece5")), /*Front deltoid*/
            new MuscleExerciseFilter(Guid.Parse("57ebdabc-7507-4b2b-8322-dc3da9b58a8c")), /*Lateral deltoid*/
            new MuscleExerciseFilter(Guid.Parse("70da1002-465a-4e77-93df-7ddd9ee21e1e")), /*Rear deltoid*/
            new MuscleGroupExerciseFilter("Chest"),
            new MuscleGroupExerciseFilter("Back"),
            new MuscleGroupExerciseFilter("Triceps"),
            new MuscleGroupExerciseFilter("Biceps")
        },
        IncludeCore = false,
        ShuffleStartIndex = 2
    };

    public static ExerciseProfile ChestDay = new ExerciseProfile
    {
        Name = "Chest day",
        ExerciseFilters = new MuscleGroupExerciseFilter[] { "Chest", "Chest", "Back", "Shoulder", "Triceps", "Biceps" },
        IncludeCore = false,
        ShuffleStartIndex = 1
    };

    public static IEnumerable<ExerciseProfile> Profiles = new[] { UpperBody, LowerBody, ShouldersDay, ChestDay };
}
