namespace WorkoutTracker.Data.Actions;

public class DeleteExerciseAction : TrackableAction<Guid>
{
    private readonly IWorkoutRepository _repository;

    public DeleteExerciseAction(IWorkoutRepository repository, ApplicationContext<DeleteExerciseAction> context)
        : base(context, "Deleting exercise")
    {
        _repository = repository;
    }

    protected override async Task Execute(IDispatcher dispatcher, Guid id, Dictionary<string, string> trackableProperties)
    {
        await _repository.DeleteExercise(id);
        await dispatcher.Dispatch<ReFetchExercisesAction>();
        Context.ShowToast("Exercise deleted.");
    }
}
