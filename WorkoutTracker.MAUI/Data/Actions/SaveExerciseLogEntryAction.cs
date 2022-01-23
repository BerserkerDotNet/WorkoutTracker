using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class SaveExerciseLogEntryAction : IAsyncAction<LogEntryViewModel>
    {
        private readonly IWorkoutRepository _repository;
        private readonly INotificationService _notificationService;

        public SaveExerciseLogEntryAction(IWorkoutRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public async Task Execute(IDispatcher dispatcher, LogEntryViewModel record)
        {
            await _repository.AddLogRecord(record);
            dispatcher.Dispatch(new AddExerciseLogEntryAction(record));
            _notificationService.ShowToast("Log entry saved.");
        }
    }
}
