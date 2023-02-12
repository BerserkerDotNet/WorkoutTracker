using WorkoutTracker.MAUI.Interfaces;

namespace WorkoutTracker.MAUI.Services;

public class NullNotificationService : INotificationService
{
    public void ShowError(string message)
    {
    }

    public void ShowToast(string message)
    {
    }
}
