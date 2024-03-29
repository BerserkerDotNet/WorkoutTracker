using Microsoft.AspNetCore.Components;

namespace WorkoutTracker.Components.Workout;

public partial class ExerciseTimerBadge : IDisposable
{
    private Color _chipColor = Color.Info;
    private string _currentTimeFormatted = "Press to start";

    [Inject]
    public IExerciseTimerService Timer { get; set; }

    [Parameter]
    public TimeSpan TargetRestTime { get; set; } = TimeSpan.MaxValue;

    public void Dispose()
    {
        Timer.OnTick -= OnTimerTick;
    }

    protected override void OnInitialized()
    {
        Timer.OnTick += OnTimerTick;
    }

    private void OnTimerTick(object sender, TimerTickEventArgs args)
    {
        var isResting = args.Mode == ExerciseTimerMode.Resting;
        _chipColor = isResting ? Color.Success : Color.Secondary;
        _chipColor = isResting && TargetRestTime < args.CurrentTime ? Color.Warning : _chipColor;
        _currentTimeFormatted = args.CurrentTime.ToString(@"mm\:ss");

        InvokeAsync(() => StateHasChanged());
    }

    private void OnStartTimer()
    {
        if (!Timer.IsRunning)
        {
            Timer.Start();
        }
    }
}