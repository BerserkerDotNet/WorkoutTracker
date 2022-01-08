using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class SaveExerciseAction : IAsyncAction<Exercise>
    {
        private readonly IRepository _repository;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;

        public SaveExerciseAction(IRepository repository, ICacheService cacheService, INotificationService notificationService)
        {
            _repository = repository;
            _cacheService = cacheService;
            _notificationService = notificationService;
        }

        public async Task Execute(IDispatcher dispatcher, Exercise exercise)
        {
            await _repository.Update(exercise);
            _cacheService.ResetExercisesCache();
            await dispatcher.Dispatch<FetchExercisesAction>();
            _notificationService.ShowToast("Exercise updated.");
        }
    }
}
