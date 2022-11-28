namespace WorkoutTracker.Data.Actions;

public class ReFetchExercisesAction : TrackableAction
{
    private readonly ICacheService _cacheService;

    public ReFetchExercisesAction(ICacheService cacheService, ApplicationContext<ReFetchExercisesAction> context)
        : base(context)
    {
        _cacheService = cacheService;
    }

    protected override async Task Execute(IDispatcher dispatcher, Dictionary<string, string> trackableProperties)
    {
        await _cacheService.ResetExercisesCache();
        await dispatcher.Dispatch<FetchExercisesAction>();
    }
}