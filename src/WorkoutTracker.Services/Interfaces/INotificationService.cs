﻿namespace WorkoutTracker.Services.Interfaces;

public interface INotificationService
{
    void ShowToast(string message);

    void ShowError(string message);
}
