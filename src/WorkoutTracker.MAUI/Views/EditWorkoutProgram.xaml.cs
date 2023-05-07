using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.MAUI.Views;

public partial class EditWorkoutProgram : ContentPage
{
    private readonly EditWorkoutProgramViewModel _viewModel;

    public EditWorkoutProgram(EditWorkoutProgramViewModel vm)
    {
        _viewModel = vm;
        InitializeComponent();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.Refresh();
    }
}