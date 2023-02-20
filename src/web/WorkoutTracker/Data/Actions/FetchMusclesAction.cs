namespace WorkoutTracker.Data.Actions;

public class FetchMusclesAction : TrackableAction
{
    private readonly IWorkoutRepository _repository;

    public FetchMusclesAction(IWorkoutRepository repository, ApplicationContext<FetchMusclesAction> context)
        : base(context)
    {
        _repository = repository;
    }
    protected override async Task Execute(IDispatcher dispatcher, Dictionary<string, string> trackableProperties)
    {
        var muscles = await _repository.GetMuscles();
        dispatcher.Dispatch(new ReceiveMusclesAction(muscles));
        trackableProperties.Add("MusclesCount", muscles.Count().ToString());
    }
}
