namespace WorkoutTracker.Data.Services
{
    public class MudNotificationService : INotificationService
    {
        private readonly ISnackbar _snackbar;

        public MudNotificationService(ISnackbar snackbar)
        {
            _snackbar = snackbar;
        }

        public void ShowError(string message)
        {
            _snackbar.Add(message, Severity.Error, cfg =>
            {
                cfg.ShowCloseIcon = true;
                cfg.VisibleStateDuration = int.MaxValue;
            });
        }

        public void ShowToast(string message)
        {
            _snackbar.Add(message, Severity.Success);
        }
    }
}
