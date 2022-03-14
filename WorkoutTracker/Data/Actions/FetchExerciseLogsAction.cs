using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.Data.Actions
{
    public class FetchExerciseLogsAction : IAsyncAction<DateTime>
    {
        private readonly IWorkoutRepository _repository;

        public FetchExerciseLogsAction(IWorkoutRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(IDispatcher dispatcher, DateTime date)
        {
            var dateKey = DateOnly.FromDateTime(date);
            var logEntryChunk = await _repository.GetLogs(date);
            dispatcher.Dispatch(new ReceiveExerciseLogsAction(dateKey, logEntryChunk.OrderByDescending(i => i.Date)));
        }
    }
}
