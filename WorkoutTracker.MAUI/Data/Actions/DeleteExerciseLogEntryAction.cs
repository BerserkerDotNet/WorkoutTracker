using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class DeleteExerciseLogEntryAction : IAsyncAction<Guid>
    {
        private readonly IWorkoutRepository _repository;
        private readonly INotificationService _notificationService;

        public DeleteExerciseLogEntryAction(IWorkoutRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public async Task Execute(IDispatcher dispatcher, Guid id)
        {
            await _repository.DeleteLog(id);
            _notificationService.ShowToast("Log entry deleted.");
            await dispatcher.Dispatch<FetchExerciseLogsAction>();
        }
    }
}
