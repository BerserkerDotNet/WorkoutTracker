namespace WorkoutTracker.Data.Actions;

public class DeleteExerciseLogEntryAction : TrackableAction<Guid>
{
    private readonly IWorkoutRepository _repository;

    public DeleteExerciseLogEntryAction(IWorkoutRepository repository, ApplicationContext<DeleteExerciseLogEntryAction> context)
        : base(context, "Deleting log entry")
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, Guid id, Dictionary<string, string> trackableProperties)
    {
        await _repository.DeleteLog(id);
        Context.ShowToast("Log entry deleted.");
        await dispatcher.Dispatch<FetchExerciseLogsAction, DateTime>(DateTime.Today);
    }
}
