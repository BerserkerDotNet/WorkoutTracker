using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Threading;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.MAUI.Services;

public class CommunityToolkitNotificationService : INotificationService
{
    public void ShowError(string message)
    {
        string actionButtonText = "Dismiss";
        var duration = TimeSpan.FromSeconds(3);

        var snackbar = Snackbar.Make(message, actionButtonText: actionButtonText, duration: TimeSpan.FromDays(1));

        snackbar.Show(CancellationToken.None);
    }

    public void ShowToast(string message)
    {
        var duration = ToastDuration.Short;
        var fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);
        toast.Show(CancellationToken.None);
    }
}