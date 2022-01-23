using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class SaveExerciseAction : IAsyncAction<ExerciseViewModel>
    {
        private readonly IWorkoutRepository _repository;
        private readonly INotificationService _notificationService;

        public SaveExerciseAction(IWorkoutRepository repository, INotificationService notificationService)
        {
            _repository = repository;
            _notificationService = notificationService;
        }

        public async Task Execute(IDispatcher dispatcher, ExerciseViewModel exercise)
        {
            await _repository.UpdateExercise(exercise);
            await dispatcher.Dispatch<FetchExercisesAction>();
            _notificationService.ShowToast("Exercise updated.");
        }
    }
}
