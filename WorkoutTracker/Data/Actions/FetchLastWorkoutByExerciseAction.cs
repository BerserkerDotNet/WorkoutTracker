namespace WorkoutTracker.Data.Actions;

public class FetchLastWorkoutByExerciseAction : TrackableAction<Guid>
{
    private readonly IWorkoutRepository _repository;

    public FetchLastWorkoutByExerciseAction(IWorkoutRepository repository, ApplicationContext<FetchLastWorkoutByExerciseAction> context)
        : base(context)
    {
        _repository = repository;
        NoBusyIndicator = true;
    }

    protected override async Task Execute(IDispatcher dispatcher, Guid exerciseId, Dictionary<string, string> trackableProperties)
    {
        var logEntry = await _repository.GetPreviousWorkoutStatsBy(exerciseId);
        dispatcher.Dispatch(new ReceiveLastWorkoutLogByExerciseAction(exerciseId, logEntry));
    }
}
