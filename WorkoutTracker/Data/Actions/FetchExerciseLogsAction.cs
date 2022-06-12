namespace WorkoutTracker.Data.Actions;

public class FetchExerciseLogsAction : TrackableAction<DateTime>
{
    private readonly IWorkoutRepository _repository;

    public FetchExerciseLogsAction(IWorkoutRepository repository, ApplicationContext<FetchExerciseLogsAction> context)
        : base(context)
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, DateTime date, Dictionary<string, string> trackableProperties)
    {
        var dateKey = DateOnly.FromDateTime(date);
        var logEntryChunk = await _repository.GetLogs(date.ToUniversalTime());
        dispatcher.Dispatch(new ReceiveExerciseLogsAction(dateKey, logEntryChunk.OrderByDescending(i => i.Date)));
    }
}