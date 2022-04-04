using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WorkoutTracker.Data
{
    public class CachedWorkoutRepository : CosmosDbWorkoutRepository // Inheritance because internally Cosmos repo needs cached exercises
    {
        private readonly ICacheService _cacheService;

        public CachedWorkoutRepository(IHttpClientFactory clientFactory, ICacheService cacheService, IAccessTokenProvider accessTokenProvider)
            : base(clientFactory, accessTokenProvider)
        {
            _cacheService = cacheService;
        }

        public override async Task<IEnumerable<ExerciseViewModel>> GetExercises()
        {
            if (_cacheService.IsExercisesCached()) 
            {
                return await _cacheService.GetExercises();
            }

            var exercises = await base.GetExercises();
            await _cacheService.SaveExercises(exercises);

            return exercises;
        }

        public override Task UpdateExercise(EditExerciseViewModel exercise) => base.UpdateExercise(exercise); // TOOD: update cache
    }
}