using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class FetchExerciseLogsAction : IAsyncAction
    {
        private readonly IExerciseLogRepository _repository;

        public FetchExerciseLogsAction(IExerciseLogRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(IDispatcher dispatcher)
        {
            var availableDatesString = await _repository.GetDates();
            var availableDates = availableDatesString
                .Select(d => DateTime.ParseExact(d, "dd-MM-yyyy", null))
                .OrderByDescending(d => d)
                .ToArray();

            if (availableDates.Length == 0)
            {
                dispatcher.Dispatch(new ReceiveExerciseLogsAction(Enumerable.Empty<ExerciseLogEntry>()));
                return;
            }

            var logEntryChunk = await _repository.GetByDate(availableDates[0]);
            dispatcher.Dispatch(new ReceiveExerciseLogsAction(logEntryChunk.OrderByDescending(i => i.Date)));
        }
    }
}
