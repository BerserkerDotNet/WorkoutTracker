using WorkoutTracker.MAUI.ViewModels;

namespace WorkoutTracker.MAUI.Views;

public partial class Library : ContentPage
{
    private readonly LibraryViewModel _viewModel;

    public Library(LibraryViewModel viewModel)
    {
        this.InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        if (!_viewModel.IsInitialized)
        {
            _viewModel.LoadExercisesCommand.Execute(this);
        }

        base.OnAppearing();
    }
}