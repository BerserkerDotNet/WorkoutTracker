namespace WorkoutTracker
{
    public interface ICacheService : IExercisesCache, IWorkoutSummariesCache
    {
    }

    public interface IExercisesCache
    {
        Task<bool> IsExercisesCached();

        Task<IEnumerable<ExerciseViewModel>> GetExercises();

        Task SaveExercises(IEnumerable<ExerciseViewModel> exercises);

        Task ResetExercisesCache();
    }

    public interface IWorkoutSummariesCache
    {
        Task<bool> IsSummariesCached();

        Task<IEnumerable<WorkoutSummary>> GetSummaries();

        Task SaveSummaries(IEnumerable<WorkoutSummary> summaries);

        Task ResetSummariesCache();
    }
}