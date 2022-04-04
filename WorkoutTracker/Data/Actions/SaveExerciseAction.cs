namespace WorkoutTracker.Data.Actions;

public class SaveExerciseAction : IAsyncAction<EditExerciseViewModel>
{
    private readonly IWorkoutRepository _repository;
    private readonly INotificationService _notificationService;

    public SaveExerciseAction(IWorkoutRepository repository, INotificationService notificationService)
    {
        _repository = repository;
        _notificationService = notificationService;
    }

    public async Task Execute(IDispatcher dispatcher, EditExerciseViewModel exercise)
    {
        await _repository.UpdateExercise(exercise);
        await dispatcher.Dispatch<FetchExercisesAction>();
        _notificationService.ShowToast("Exercise updated.");
    }
}