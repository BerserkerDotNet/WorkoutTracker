using System;
using System.Threading;
using System.Threading.Tasks;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.MAUI.Services;



public class ExerciseTimerService : IExerciseTimerService, IDisposable
{
    private TimeSpan _currentTime = TimeSpan.Zero;
    private ExerciseTimerMode _currentMode;
    private CancellationTokenSource _source;

    public TimeSpan CurrentTime => _currentTime;

    public ExerciseTimerMode CurrentMode => _currentMode;

    public bool IsRunning => _source is not null && !_source.IsCancellationRequested;

    public event TimerTickEventHandler OnTick;

    public void Dispose()
    {
        _source?.Cancel();
        _source?.Dispose();
    }

    private async Task StartCounting(CancellationToken token)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
        while (await timer.WaitForNextTickAsync())
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            _currentTime += TimeSpan.FromSeconds(1);
            OnTick?.Invoke(this, new TimerTickEventArgs(_currentTime, _currentMode));
        }
    }

    public void Start()
    {
        _source = new CancellationTokenSource();
        Task.Factory.StartNew(async () => await StartCounting(_source.Token), TaskCreationOptions.LongRunning);
    }

    public void Stop()
    {
        _source?.Cancel();
    }

    public void SetMode(ExerciseTimerMode mode)
    {
        _currentMode = mode;
        _currentTime = TimeSpan.Zero;
        OnTick?.Invoke(this, new TimerTickEventArgs(_currentTime, _currentMode));
    }
}