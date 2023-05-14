using AndroidX.Work;
using System;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.MAUI.Views;

public partial class Workout : ContentPage
{
    private readonly WorkoutViewModel _viewModel;

    public Workout(WorkoutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadExercisesCommand.Execute(null);
        _viewModel.GetOrCreateWorkoutCommand.Execute(null);
    }

    private void Button_Clicked(object sender, System.EventArgs e)
    {
        using var builder = new Constraints.Builder();
        builder.SetRequiredNetworkType(NetworkType.Connected);
        var workConstraints = builder.Build();

        var workerRequest = new PeriodicWorkRequest.Builder(typeof(DataSyncWorker), TimeSpan.FromMinutes(15))
            .SetConstraints(workConstraints)
            .AddTag(DataSyncWorker.TAG)
            .Build();

        WorkManager.GetInstance(Microsoft.Maui.ApplicationModel.Platform.AppContext)
            .EnqueueUniquePeriodicWork(DataSyncWorker.TAG, ExistingPeriodicWorkPolicy.Keep, workerRequest);
    }
}