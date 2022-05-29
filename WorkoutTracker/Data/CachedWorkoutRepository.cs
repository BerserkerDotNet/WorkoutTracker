using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;

namespace WorkoutTracker.Data;

public class CachedWorkoutRepository : CosmosDbWorkoutRepository // Inheritance because internally Cosmos repo needs cached exercises
{
    private readonly ICacheService _cacheService;

    public CachedWorkoutRepository(IHttpClientFactory clientFactory, ICacheService cacheService, IAccessTokenProvider accessTokenProvider, ApplicationContext<IWorkoutRepository> context)
        : base(clientFactory, accessTokenProvider, context)
    {
        _cacheService = cacheService;
    }

    public override async Task<IEnumerable<ExerciseViewModel>> GetExercises()
    {
        if (await _cacheService.IsExercisesCached()) 
        {
            Context.LogInformation("Cache hit, returning cached exercises.");
            return await _cacheService.GetExercises();
        }

        Context.LogWarning("Cache miss, fetching exercises from server.");
        var exercises = await base.GetExercises();
        await _cacheService.SaveExercises(exercises);

        return exercises;
    }

    public override Task UpdateExercise(EditExerciseViewModel exercise) => base.UpdateExercise(exercise); // TOOD: update cache
}