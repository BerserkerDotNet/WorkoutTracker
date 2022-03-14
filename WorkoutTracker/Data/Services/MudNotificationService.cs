namespace WorkoutTracker.Data.Services
{
    public class MudNotificationService : INotificationService
    {
        private readonly ISnackbar _snackbar;

        public MudNotificationService(ISnackbar snackbar)
        {
            _snackbar = snackbar;
        }

        public void ShowToast(string message)
        {
            _snackbar.Add(message);
        }
    }
}
