using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Components
{
	public partial class StartStopButton
	{
		private bool _isRunning;

		[Parameter]
		public string IdleText { get; set; }

		[Parameter]
		public string ActiveText { get; set; }

		[Parameter]
        public EventCallback OnStart { get; set; }

		[Parameter]
        public EventCallback OnEnd { get; set; }

        private async Task OnClick()
        {
            if (!_isRunning)
            {
                _isRunning = true;
                await OnStart.InvokeAsync();
            }
            else
            {
                _isRunning = false;
                await OnEnd.InvokeAsync();
            }
        }
    }
}