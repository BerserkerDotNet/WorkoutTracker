namespace WorkoutTracker.Data.Actions
{
    public class FetchMusclesAction : IAsyncAction
    {
        private readonly IWorkoutRepository _repository;

        public FetchMusclesAction(IWorkoutRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(IDispatcher dispatcher)
        {
            var muscles = await _repository.GetMuscles();
            dispatcher.Dispatch(new ReceiveMusclesAction(muscles));
        }
    }
}
