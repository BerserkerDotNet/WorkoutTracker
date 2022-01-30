using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Data.Actions
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
            var logEntryChunk = await _repository.GetLogs(date);
            dispatcher.Dispatch(new ReceiveExerciseLogsAction(DateOnly.FromDateTime(date), logEntryChunk.OrderByDescending(i => i.Date)));
        }
    }
}
