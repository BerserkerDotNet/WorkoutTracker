using DevExpress.Maui.Editors;
using System.Linq;
using WorkoutTracker.MAUI.ViewModels;

namespace WorkoutTracker.MAUI.Views;

public partial class EditExercisePage : ContentPage
{
    private readonly EditExercisePageViewModel _viewModel;

    public EditExercisePage(EditExercisePageViewModel vm)
    {
        _viewModel = vm;
        InitializeComponent();
        BindingContext = _viewModel;
    }

    private void InputChipGroup_Completed(object sender, DevExpress.Maui.Editors.CompletedEventArgs e)
    {
        var chipGroup = sender as InputChipGroup;
        _viewModel.AddTagCommand.Execute(chipGroup.EditorText);
    }

    private void InputChipGroup_ChipRemoveIconClicked(object sender, ChipEventArgs e)
    {

    }

    private void AsyncItemsSourceProvider_ItemsRequested(object sender, ItemsRequestEventArgs e)
    {
        if (!_viewModel.IsInitialized)
        {
            _viewModel.LoadMuscles();
        }

        e.Request = () => this._viewModel.Muscles.Where(m => m.Name.Contains(e.Text, System.StringComparison.OrdinalIgnoreCase)).ToList();
    }
}