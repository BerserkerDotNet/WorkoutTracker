using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.MAUI.Views;

public partial class WorkoutStats : ContentPage
{
    private readonly WorkoutStatsViewModel _vm;

    public WorkoutStats(WorkoutStatsViewModel vm)
    {
        _vm = vm;
        InitializeComponent();
        BindingContext = _vm;
    }
        
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _vm.Init();
    }
}