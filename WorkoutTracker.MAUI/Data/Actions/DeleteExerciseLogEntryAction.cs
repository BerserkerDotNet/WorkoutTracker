using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class DeleteExerciseLogEntryAction : IAsyncAction<Guid>
    {
        private readonly IExerciseLogRepository _repository;
        private readonly INotificationService _notificationService;

        public DeleteExerciseLogEntryAction(IExerciseLogRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public async Task Execute(IDispatcher dispatcher, Guid id)
        {
            await _repository.Delete<ExerciseLogEntry>(id);
            _notificationService.ShowToast("Log entry deleted.");
            await dispatcher.Dispatch<FetchExerciseLogsAction>();
        }
    }
}
