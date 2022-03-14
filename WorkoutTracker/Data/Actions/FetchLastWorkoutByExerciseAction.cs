using System.Threading.Tasks;

namespace WorkoutTracker.Data.Actions
{
    public class FetchLastWorkoutByExerciseAction : IAsyncAction<Guid>
    {
        private readonly IWorkoutRepository _repository;

        public FetchLastWorkoutByExerciseAction(IWorkoutRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(IDispatcher dispatcher, Guid exerciseId)
        {
            var logEntry = await _repository.GetPreviousWorkoutStatsBy(exerciseId);
            dispatcher.Dispatch(new ReceiveLastWorkoutLogByExerciseAction(exerciseId, logEntry));
        }
    }
}
