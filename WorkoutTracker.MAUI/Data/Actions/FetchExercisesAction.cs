using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class FetchExercisesAction : IAsyncAction
    {
        private readonly IExerciseLogRepository _repository;
        private readonly ICacheService _cacheService;

        public FetchExercisesAction(IExerciseLogRepository repository, ICacheService cacheService)
        {
            _repository = repository;
            _cacheService = cacheService;
        }

        public async Task Execute(IDispatcher dispatcher)
        {
            if (_cacheService.IsExercisesCached())
            {
                var exercises = await _cacheService.GetExercises();
                dispatcher.Dispatch(new ReceiveExercisesAction(exercises));
            }
            else
            {
                var exercises = await _repository.GetAll<Exercise>();
                await _cacheService.SaveExercises(exercises);
                dispatcher.Dispatch(new ReceiveExercisesAction(exercises));
            }
        }
    }
}
