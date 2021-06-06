using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class SaveExerciseLogEntryAction : IAsyncAction<ExerciseLogEntry>
    {
        private readonly IExerciseLogRepository _repository;
        private readonly INotificationService _notificationService;

        public SaveExerciseLogEntryAction(IExerciseLogRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public async Task Execute(IDispatcher dispatcher, ExerciseLogEntry entry)
        {
            await _repository.Create(entry);
            dispatcher.Dispatch(new AddExerciseLogEntryAction(entry));
            _notificationService.ShowToast("Log entry saved.");
        }
    }
}
