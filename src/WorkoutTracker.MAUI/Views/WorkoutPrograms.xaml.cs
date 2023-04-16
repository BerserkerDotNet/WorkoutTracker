using WorkoutTracker.MAUI.ViewModels;

namespace WorkoutTracker.MAUI.Views;

public partial class WorkoutPrograms : ContentPage
{
    private readonly WorkoutProgramsViewModel _viewModel;

    public WorkoutPrograms(WorkoutProgramsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _viewModel.LoadProgramsCommand.Execute(null);
    }
}