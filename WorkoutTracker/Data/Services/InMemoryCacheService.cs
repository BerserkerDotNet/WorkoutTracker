using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WorkoutTracker.Data.Services
{
    public class InMemoryCacheService : ICacheService
    {
        private IEnumerable<ExerciseViewModel> _exercisesInMemoryCache;
        private AccessToken _token;

        public Task SaveExercises(IEnumerable<ExerciseViewModel> exercises)
        {
            _exercisesInMemoryCache = exercises;

            return Task.CompletedTask;
        }

        public Task<IEnumerable<ExerciseViewModel>> GetExercises()
        {
            if (_exercisesInMemoryCache is object)
            {
                return Task.FromResult(_exercisesInMemoryCache);
            }

            return Task.FromResult(Enumerable.Empty<ExerciseViewModel>());
        }

        public void ResetExercisesCache()
        {
            _exercisesInMemoryCache = null;
        }

        public bool IsExercisesCached()
        {
            return _exercisesInMemoryCache is object;
        }

        public Task<AccessToken> GetToken()
        {
            return Task.FromResult(_token);
        }

        public Task SaveToken(AccessToken token)
        {
            _token = token;
            return Task.CompletedTask;
        }
    }
}
