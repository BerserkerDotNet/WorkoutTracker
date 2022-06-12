using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WorkoutTracker.Data.Services
{
    public class InMemoryCacheService : ICacheService
    {
        private IEnumerable<ExerciseViewModel> _exercisesInMemoryCache;
        private IEnumerable<WorkoutSummary> _summariesInMemoryCache;

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

        public Task ResetExercisesCache()
        {
            _exercisesInMemoryCache = null;

            return Task.CompletedTask;
        }

        public Task<bool> IsExercisesCached()
        {
            return Task.FromResult(_exercisesInMemoryCache is object);
        }

        public Task<bool> IsSummariesCached()
        {
            return Task.FromResult(_summariesInMemoryCache is object);
        }

        public Task<IEnumerable<WorkoutSummary>> GetSummaries()
        {
            return Task.FromResult(_summariesInMemoryCache);
        }

        public Task SaveSummaries(IEnumerable<WorkoutSummary> summaries)
        {
            _summariesInMemoryCache = summaries;
            return Task.CompletedTask;
        }

        public Task ResetSummariesCache()
        {
            _summariesInMemoryCache = null;
            return Task.CompletedTask;
        }
    }
}
