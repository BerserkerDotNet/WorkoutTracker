public class ExerciseProfile
{
    public static int DefaultNumberOfSets = 4;
    public static TimeSpan DefaultRestTime = TimeSpan.FromMinutes(2);

    public string Name { get; set; }

    public string Description { get; set; }

    public IExerciseSelector[] ExerciseSelectors { get; set; }

    public bool IncludeCore { get; set; }

    public IShuffle Shuffle { get; set; } = new ShuffleAll();

    public int NumberOfSets { get; set; } = DefaultNumberOfSets;

    public TimeSpan RestTime { get; set; } = DefaultRestTime;

    public static ExerciseProfile UpperBody = new ExerciseProfile
    {
        Name = "Upper Body",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Chest", "Back", "Shoulder", "Triceps", "Biceps" },
        IncludeCore = true
    };

    public static ExerciseProfile BackDay = new ExerciseProfile
    {
        Name = "Back Day",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Back", "Back", "Back", "Biceps", "Biceps" },
        Shuffle = new NoShuffle()
    };

    public static ExerciseProfile ChestDay = new ExerciseProfile
    {
        Name = "Chest Day",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Chest", "Chest", "Chest", "Triceps", "Shoulder" },
        Shuffle = new NoShuffle()
    };

    public static ExerciseProfile ShouldersDay = new ExerciseProfile
    {
        Name = "Shoulders Day",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Shoulder", "Shoulder", "Shoulder", "Chest", "Chest" },
        Shuffle = new NoShuffle()
    };

    public static ExerciseProfile LegsAndTricepsDay = new ExerciseProfile
    {
        Name = "Legs and Triceps Day",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Quads", "Quads", "Glutes", "Triceps", "Triceps" },
        Shuffle = new NoShuffle()
    };

    public static ExerciseProfile LegsAndBicepsDay = new ExerciseProfile
    {
        Name = "Legs and Biceps Day",
        ExerciseSelectors = new MuscleGroupExerciseSelector[] { "Quads", "Hamstrings", "Glutes", "Biceps", "Biceps" },
        Shuffle = new NoShuffle()
    };

    public static ExerciseProfile SquadsLowerBody = new ExerciseProfile
    {
        Name = "Lower Body Squads",
        ExerciseSelectors = new IExerciseSelector[]
        {
            new SpecificExerciseSelector(Guid.Parse("b71b20c9-0fb4-4351-aa6f-41cffc75daed"), TargetSets: 5), /*Barbell Squats*/
            new SpecificExerciseSelector(Guid.Parse("6d72eb3d-2a43-4d88-9b32-e0db6946148a")), /*Bulgarian Split Squats*/
            new SpecificExerciseSelector(Guid.Parse("5fded951-3796-414f-b28e-16e4d0674fda")), /*Barbell Hip Thrust*/
            new SpecificExerciseSelector(Guid.Parse("a7b4d27f-568d-4dc3-9086-8a3c01d66904")), /*Leg press*/
            new MuscleGroupExerciseSelector("Calves"),
        },
        IncludeCore = false,
        Shuffle = new NoShuffle(),
        RestTime = TimeSpan.FromMinutes(3),
    };

    public static ExerciseProfile DeadliftLowerBody = new ExerciseProfile
    {
        Name = "Lower Body Deadlift",
        ExerciseSelectors = new IExerciseSelector[]
        {
            new SpecificExerciseSelector(Guid.Parse("b33af80a-8608-4ef9-a792-a18c7eadb676"), TargetSets: 5), /*Deadlift*/
            new MuscleGroupExerciseSelector("Hamstrings"),
            new SpecificExerciseSelector(Guid.Parse("080ac468-8b00-4bb5-b21a-90ee8b45a568")), /*Dumbbell lunge*/
            new SpecificExerciseSelector(Guid.Parse("8afdd840-66b6-4810-ba79-fa7d33e0ae0d")), /*Leg Extension*/
        },
        IncludeCore = false,
        Shuffle = new NoShuffle(),
        RestTime = TimeSpan.FromMinutes(3),
    };

    public static IEnumerable<ExerciseProfile> Profiles = new[]
    {
        UpperBody,
        BackDay,
        ChestDay,
        ShouldersDay,
        LegsAndTricepsDay,
        LegsAndBicepsDay
    };

    public static ExerciseProfile GetDefaultProfile()
    {
        switch (DateTime.Today.DayOfWeek)
        {
            case DayOfWeek.Monday:
                return BackDay;
            case DayOfWeek.Tuesday:
                return LegsAndTricepsDay;
            case DayOfWeek.Wednesday:
                return ChestDay;
            case DayOfWeek.Thursday:
                return LegsAndBicepsDay;
            case DayOfWeek.Friday:
                return ShouldersDay;
            default:
                return UpperBody;
        }
    }
}
