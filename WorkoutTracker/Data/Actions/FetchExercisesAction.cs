using System.Threading.Tasks;

namespace WorkoutTracker.Data.Actions
{
    public class FetchExercisesAction : IAsyncAction
    {
        private readonly IWorkoutRepository _repository;

        public FetchExercisesAction(IWorkoutRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(IDispatcher dispatcher)
        {
            var exercises = await _repository.GetExercises();
            dispatcher.Dispatch(new ReceiveExercisesAction(exercises));
        }
    }
}
