using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Data
{
    public class CachedWorkoutRepository : CosmosDbWorkoutRepository // Inheritance because internally Cosmos repo needs cached exercises
    {
        private readonly ICacheService _cacheService;

        public CachedWorkoutRepository(IHttpClientFactory clientFactory, ICacheService cacheService)
            : base(clientFactory)
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

        public override Task UpdateExercise(ExerciseViewModel exercise) => base.UpdateExercise(exercise); // TOOD: update cache
    }
}