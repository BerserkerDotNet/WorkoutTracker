public class ExerciseProfile
{
    public string Name { get; set; }

    public string Description { get; set; }

    public IExerciseSelector[] ExerciseSelectors { get; set; }

    public bool IncludeCore { get; set; }

    public IShuffle Shuffler { get; set; } = new ShuffleAll();

    public int DefaultNumberOfSets { get; set; } = 5;

    public TimeSpan DefaultRestTime { get; set; } = TimeSpan.FromMinutes(2);

    public static ExerciseProfile UpperBody = new ExerciseProfile
    {
        Name = "Upper Body",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Chest", "Back", "Shoulder", "Triceps", "Biceps" },
        IncludeCore = true
    };

    public static ExerciseProfile ChestFirstUpperBody = new ExerciseProfile
    {
        Name = "Chest First Upper Body",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Chest", "Back", "Shoulder", "Triceps", "Biceps" },
        IncludeCore = false,
        Shuffler = new IndexBasedShuffle(1)
    };

    public static ExerciseProfile ShouldersFirstUpperBody = new ExerciseProfile
    {
        Name = "Shoulders First Upper Body",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Shoulder", "Chest", "Back", "Triceps", "Biceps" },
        Shuffler = new IndexBasedShuffle(1),
        IncludeCore = false
    };

    public static ExerciseProfile BicepsFirstUpperBody = new ExerciseProfile
    {
        Name = "Biceps First Upper Body",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Biceps", "Chest", "Back", "Shoulder", "Triceps" },
        Shuffler = new IndexBasedShuffle(1),
        IncludeCore = false
    };

    public static ExerciseProfile BackFirstUpperBody = new ExerciseProfile
    {
        Name = "Back First Upper Body",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Back", "Chest", "Shoulder", "Triceps", "Biceps" },
        Shuffler = new IndexBasedShuffle(1),
        IncludeCore = false
    };

    public static ExerciseProfile SquadsLowerBody = new ExerciseProfile
    {
        Name = "Lower Body Squads",
        ExerciseSelectors = new IExerciseSelector[]
        {
            new SpecificExerciseSelector(Guid.Parse("b71b20c9-0fb4-4351-aa6f-41cffc75daed")), /*Barbell Squats*/
            new MuscleGroupExerciseSelector("Glutes"),
            new MuscleGroupExerciseSelector("Glutes"),
            new MuscleGroupExerciseSelector("Calves"),
        },
        IncludeCore = false,
        Shuffler = new IndexBasedShuffle(1),
        DefaultRestTime = TimeSpan.FromMinutes(3),
    };

    public static ExerciseProfile DeadliftLowerBody = new ExerciseProfile
    {
        Name = "Lower Body Deadlift",
        ExerciseSelectors = new IExerciseSelector[]
        {
            new SpecificExerciseSelector(Guid.Parse("b33af80a-8608-4ef9-a792-a18c7eadb676")), /*Deadlift*/
            new MuscleGroupExerciseSelector("Hamstrings"),
            new MuscleGroupExerciseSelector("Quads"),
            new MuscleGroupExerciseSelector("Quads")
        },
        IncludeCore = false,
        Shuffler = new IndexBasedShuffle(1),
        DefaultRestTime = TimeSpan.FromMinutes(3),
    };

    public static ExerciseProfile DoubleUpperBody = new ExerciseProfile
    {
        Name = "Double upper body",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Chest", "Chest", "Back", "Back", "Shoulder", "Shoulder", "Triceps", "Triceps", "Biceps", "Biceps" },
        IncludeCore = false,
        DefaultNumberOfSets = 4,
        Shuffler = new GroupShuffle(0, 2)
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

    public static IEnumerable<ExerciseProfile> Profiles = new[]
    {
        UpperBody,
        ChestFirstUpperBody,
        ShouldersFirstUpperBody,
        BackFirstUpperBody,
        BicepsFirstUpperBody,
        SquadsLowerBody,
        DeadliftLowerBody,
        DoubleUpperBody,
        ShouldersDay,
        ChestDay,
        BicepsDay
    };
}
