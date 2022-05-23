public class ExerciseProfile
{
    public string Name { get; set; }

    public string Description { get; set; }

    public IExerciseSelector[] ExerciseSelectors { get; set; }

    public bool IncludeCore { get; set; }

    public IShuffle Shuffler { get; set; } = new ShuffleAll();

    public int DefaultNumberOfSets { get; set; } = 5;

    public TimeSpan DefaultRestTime { get; set; } = TimeSpan.FromMinutes(1);

    public static ExerciseProfile UpperBody = new ExerciseProfile
    {
        Name = "Upper body",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Chest", "Back", "Shoulder", "Triceps", "Biceps" },
        IncludeCore = true
    };

    public static ExerciseProfile DoubleUpperBody = new ExerciseProfile
    {
        Name = "Double upper body",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Chest", "Chest", "Back", "Back", "Shoulder", "Shoulder", "Triceps", "Triceps", "Biceps", "Biceps" },
        IncludeCore = false,
        DefaultNumberOfSets = 4,
        Shuffler = new GroupShuffle(0, 2)
    };

    public static ExerciseProfile LowerBody = new ExerciseProfile
    {
        Name = "Lower body",
        ExerciseSelectors = new IExerciseSelector[]
        {
            new SpecificExerciseSelector(Guid.Parse("b33af80a-8608-4ef9-a792-a18c7eadb676")), /*Deadlift*/
            new MuscleGroupExerciseSelector("Glutes"),
            new MuscleGroupExerciseSelector("Quads", 4),
            new MuscleGroupExerciseSelector("Quads", 4),
            new MuscleGroupExerciseSelector("Hamstrings"),
            new MuscleGroupExerciseSelector("Calves"),
            new MuscleGroupExerciseSelector("Adductors")
        },
        IncludeCore = false,
        Shuffler = new IndexBasedShuffle(1),
        DefaultRestTime = TimeSpan.FromMinutes(2),
    };

    public static ExerciseProfile ShouldersDay = new ExerciseProfile
    {
        Name = "Shoulders day",
        ExerciseSelectors = new IExerciseSelector[]
        {
            new MuscleExerciseSelector(Guid.Parse("9024dbc7-defb-4aa3-8b0d-29ce21d1ece5"), 4), /*Front deltoid*/
            new MuscleExerciseSelector(Guid.Parse("57ebdabc-7507-4b2b-8322-dc3da9b58a8c"), 4), /*Lateral deltoid*/
            new MuscleExerciseSelector(Guid.Parse("70da1002-465a-4e77-93df-7ddd9ee21e1e"), 4), /*Rear deltoid*/
            new MuscleGroupExerciseSelector("Chest"),
            new MuscleGroupExerciseSelector("Back"),
            new MuscleGroupExerciseSelector("Triceps"),
            new MuscleGroupExerciseSelector("Biceps")
        },
        IncludeCore = false,
        Shuffler = new IndexBasedShuffle(2),
    };

    public static ExerciseProfile ChestDay = new ExerciseProfile
    {
        Name = "Chest day",
        ExerciseSelectors = new MuscleGroupExerciseSelector[]
        {
            new MuscleGroupExerciseSelector("Chest", 4),
            new MuscleGroupExerciseSelector("Chest", 4),
            new MuscleGroupExerciseSelector("Chest", 4),
            new MuscleGroupExerciseSelector("Back"),
            new MuscleGroupExerciseSelector("Shoulder"),
            new MuscleGroupExerciseSelector("Triceps"),
            new MuscleGroupExerciseSelector("Biceps")
        },
        IncludeCore = false,
        Shuffler = new IndexBasedShuffle(2)
    };

    public static ExerciseProfile BicepsDay = new ExerciseProfile
    {
        Name = "Biceps day",
        ExerciseSelectors = new MuscleGroupExerciseSelector[]
        {
            new MuscleGroupExerciseSelector("Biceps",4),
            new MuscleGroupExerciseSelector("Biceps",4),
            new MuscleGroupExerciseSelector("Biceps",4),
            new MuscleGroupExerciseSelector("Back"),
            new MuscleGroupExerciseSelector("Shoulder"),
            new MuscleGroupExerciseSelector("Triceps"),
            new MuscleGroupExerciseSelector("Chest")
        },
        IncludeCore = false,
        Shuffler = new IndexBasedShuffle(2)
    };

    public static IEnumerable<ExerciseProfile> Profiles = new[] { UpperBody, DoubleUpperBody, LowerBody, ShouldersDay, ChestDay, BicepsDay };
}
