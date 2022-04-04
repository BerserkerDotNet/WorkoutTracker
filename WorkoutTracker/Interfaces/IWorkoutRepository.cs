using Microsoft.AspNetCore.Components.Forms;

namespace WorkoutTracker
{
    public interface IWorkoutRepository
    {
        Task<IEnumerable<MuscleViewModel>> GetMuscles();

        Task<IEnumerable<ExerciseViewModel>> GetExercises();

        Task<IEnumerable<LogEntryViewModel>> GetLogs(DateTime date);

        Task<LogEntryViewModel> GetPreviousWorkoutStatsBy(Guid exerciseId);

        Task AddLogRecord(LogEntryViewModel model);

        Task DeleteLog(Guid id);

        Task DeleteExercise(Guid id);

        Task UpdateExercise(EditExerciseViewModel exercise);

        Task UpdateMuscle(MuscleViewModel muscle, IBrowserFile imageFile);
    }
}