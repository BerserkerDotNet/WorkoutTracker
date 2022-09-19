namespace WorkoutTracker.Data.Actions;

public record WorkoutStatsRequest(DateTime From, DateTime To);

public class FetchWorkoutStatsAction : TrackableAction<WorkoutStatsRequest>
{
    private readonly IWorkoutRepository _repository;

    public FetchWorkoutStatsAction(IWorkoutRepository repository, ApplicationContext<FetchWorkoutStatsAction> context)
        : base(context)
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, WorkoutStatsRequest request, Dictionary<string, string> trackableProperties)
    {
        var summaries = await _repository.GetWorkoutSummaries(request.From, request.To);
        dispatcher.Dispatch(new ReceiveWorkoutStatsAction(summaries));
    }
}