namespace WorkoutTracker.Data.Actions;

public class ReFetchExercisesAction : IAsyncAction
{
    private readonly ICacheService _cacheService;
    private readonly IWorkoutRepository _repository;

    public ReFetchExercisesAction(ICacheService cacheService, IWorkoutRepository repository)
    {
        _cacheService = cacheService;
        _repository = repository;
    }

    public async Task Execute(IDispatcher dispatcher)
    {
        await _cacheService.ResetExercisesCache();
        var exercises = await _repository.GetExercises();
        dispatcher.Dispatch(new ReceiveExercisesAction(exercises));
    }
}