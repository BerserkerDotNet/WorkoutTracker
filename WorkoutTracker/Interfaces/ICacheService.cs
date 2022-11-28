using WorkoutTracker.ViewModels.Workout;

namespace WorkoutTracker;

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

public interface ISetsProvider
{
    Task<IEnumerable<WorkoutSet>> Generate(Guid exerciseId);

    Task<SetsOverloadDecision> ShouldOverload(Guid exerciseId, IEnumerable<WorkoutSet> lastSets);
}

public interface IExerciseTimerService
{
    void Start();

    void Stop();

    void SetMode(ExerciseTimerMode mode);

    bool IsRunning { get; }

    TimeSpan CurrentTime { get; }

    ExerciseTimerMode CurrentMode { get; }

    event TimerTickEventHandler OnTick;
}

public enum ExerciseTimerMode
{
    Resting,
    Exercising
}

public record struct TimerTickEventArgs(TimeSpan CurrentTime, ExerciseTimerMode Mode);

public delegate void TimerTickEventHandler(object sender, TimerTickEventArgs e);