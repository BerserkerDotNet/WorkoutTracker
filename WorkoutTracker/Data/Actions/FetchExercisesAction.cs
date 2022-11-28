namespace WorkoutTracker.Data.Actions;

public class FetchExercisesAction : TrackableAction
{
    private readonly IWorkoutRepository _repository;

    public FetchExercisesAction(IWorkoutRepository repository, ApplicationContext<FetchExercisesAction> context)
        : base(context, "Loading exercises")
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, Dictionary<string, string> trackableProperties)
    {
        var exercises = await _repository.GetExercises();
        dispatcher.Dispatch(new ReceiveExercisesAction(exercises));
        trackableProperties.Add("ExerciseCount", exercises.Count().ToString());
    }
}
