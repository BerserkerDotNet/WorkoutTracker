using Microsoft.AspNetCore.Components;
using Sharp.CSS.Blazor;
using System.Threading;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Components
{
    public partial class ExerciseDuration
    {
        private bool _isRunning = false;
        CancellationTokenSource _source;

        [Parameter]
        public int Duration { get; set; }

        [Parameter]
        public EventCallback<int> DurationChanged { get; set; }

        [Parameter]
        public string InputClass { get; set; }

        [Parameter]
        public string ButtonClass { get; set; }

        private void ToggleExerciseStopwatch()
        {
            if (!_isRunning)
            {
                _source = new CancellationTokenSource();
                Task.Factory.StartNew(async () => await StartCounting(_source.Token));
                _isRunning = true;
            }
            else
            {
                _source.Cancel();
                _source = null;
                _isRunning = false;
            }
        }

        private async Task StartCounting(CancellationToken token)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            int currentDuration = 0;
            while (await timer.WaitForNextTickAsync())
            {
                if (token.IsCancellationRequested)
                {
                    break;
                }

                Duration = ++currentDuration;
                await InvokeAsync(async () => await DurationChanged.InvokeAsync(Duration));
            }
        }
    }

    public class ExerciseDurationStyles : IStylesModule
    {
        public void Configure(SharpCssConfigurator configurator)
        {
            throw new NotImplementedException();
        }
    }
}