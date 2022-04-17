namespace WorkoutTracker.Data.Actions;

public class FetchExercisesAction : IAsyncAction
{
    private readonly ICacheService _cacheService;
    private readonly IWorkoutRepository _repository;

    public FetchExercisesAction(ICacheService cacheService, IWorkoutRepository repository)
    {
        _cacheService = cacheService;
        _repository = repository;
    }

    public async Task Execute(IDispatcher dispatcher)
    {
        _cacheService.ResetExercisesCache();
        var exercises = await _repository.GetExercises();
        dispatcher.Dispatch(new ReceiveExercisesAction(exercises));
    }
}