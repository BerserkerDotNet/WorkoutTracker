namespace WorkoutTracker.Data.Actions;

public class SaveExerciseLogEntryAction : TrackableAction<LogEntryViewModel>
{
    private readonly IWorkoutRepository _repository;

    public SaveExerciseLogEntryAction(IWorkoutRepository repository, ApplicationContext<SaveExerciseLogEntryAction> context)
        : base(context)
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, LogEntryViewModel record, Dictionary<string, string> trackableProperties)
    {
        trackableProperties.Add(nameof(record.Id), record.Id.ToString());

        await _repository.AddLogRecord(record);
        dispatcher.Dispatch(new AddExerciseLogEntryAction(record));
        Context.ShowToast("Entry saved.");
    }
}
