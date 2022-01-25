using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Data.Actions
{
    public class SaveExerciseLogEntryAction : IAsyncAction<LogEntryViewModel>
    {
        private readonly IWorkoutRepository _repository;

        public SaveExerciseLogEntryAction(IWorkoutRepository repository)
        {
            _repository = repository;
        }

        public async Task Execute(IDispatcher dispatcher, LogEntryViewModel record)
        {
            await _repository.AddLogRecord(record);
            dispatcher.Dispatch(new AddExerciseLogEntryAction(record));
        }
    }
}
