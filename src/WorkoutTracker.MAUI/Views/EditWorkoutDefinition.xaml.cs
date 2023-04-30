using WorkoutTracker.MAUI.ViewModels;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.MAUI.Views;

public partial class EditWorkoutDefinition : ContentPage
{
    private EditWorkoutDefinitionViewModel _vm;

    public EditWorkoutDefinition(EditWorkoutDefinitionViewModel vm)
    {
        _vm = vm;
        InitializeComponent();
        BindingContext = vm;
    }

    protected override void OnAppearing()
    {
        _vm.Init();
        base.OnAppearing();
    }
}