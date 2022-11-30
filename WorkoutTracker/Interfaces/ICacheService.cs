using WorkoutTracker.ViewModels.Workout;

namespace WorkoutTracker;

public interface ICacheService
{
    ValueTask<T> GetAsync<T>(string key)
        where T : class;

    ValueTask SetAsync<T>(string key, T entry)
        where T : class;

    ValueTask RemoveAsync(string key);

    ValueTask<bool> HasKey(string key);
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