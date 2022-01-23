using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class FetchExerciseLogsAction : IAsyncAction
    {
        private readonly IWorkoutRepository _repository;

        public FetchExerciseLogsAction(IWorkoutRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(IDispatcher dispatcher)
        {
            var logEntryChunk = await _repository.GetLogs();
            dispatcher.Dispatch(new ReceiveExerciseLogsAction(logEntryChunk.OrderByDescending(i => i.Date)));
        }
    }
}
