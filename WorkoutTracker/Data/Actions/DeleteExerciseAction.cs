namespace WorkoutTracker.Data.Actions
{
    public class DeleteExerciseAction : IAsyncAction<Guid>
    {
        private readonly IWorkoutRepository _repository;
        private readonly INotificationService _notificationService;

        public DeleteExerciseAction(IWorkoutRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public async Task Execute(IDispatcher dispatcher, Guid id)
        {
            await _repository.DeleteExercise(id);
            _notificationService.ShowToast("Exercise deleted.");
            await dispatcher.Dispatch<ReFetchExercisesAction>();
        }
    }
}
