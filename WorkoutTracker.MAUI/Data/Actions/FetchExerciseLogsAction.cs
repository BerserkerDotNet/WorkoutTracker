using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class FetchExerciseLogsAction : IAsyncAction
    {
        private readonly IRepository _repository;

        public FetchExerciseLogsAction(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(IDispatcher dispatcher)
        {
            var logEntryChunk = await _repository.GetAll<ExerciseLogEntry>();
            dispatcher.Dispatch(new ReceiveExerciseLogsAction(logEntryChunk.OrderByDescending(i => i.Date)));
        }
    }
}
