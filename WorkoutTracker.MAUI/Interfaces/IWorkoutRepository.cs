using System.Collections.Generic;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI
{
    public interface IWorkoutRepository
    {
        Task<IEnumerable<ExerciseViewModel>> GetExercises();

        Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime date);

        Task<LogEntryViewModel> GetPreviousWorkoutStatsBy(Guid exerciseId);

        Task AddLogRecord(LogEntryViewModel model);

        Task DeleteLog(Guid id);

        Task UpdateExercise(ExerciseViewModel exercise);
    }
}