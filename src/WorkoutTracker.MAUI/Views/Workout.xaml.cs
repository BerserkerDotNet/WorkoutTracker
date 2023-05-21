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
}