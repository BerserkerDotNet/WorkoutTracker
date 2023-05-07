using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.Json;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Services.Interfaces;
using INavigation = WorkoutTracker.Services.Interfaces.INavigation;

namespace WorkoutTracker.Services.ViewModels;

public sealed partial class EditWorkoutProgramViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private WorkoutProgram _workoutProgram;

    private readonly IWorkoutDataProvider _trackerDb;
    private readonly INavigation _navigation;

    public EditWorkoutProgramViewModel(IWorkoutDataProvider trackerDb, INavigation navigation)
    {
        _trackerDb = trackerDb;
        _navigation = navigation;
    }
    
    [RelayCommand]
    public async Task EditDefinition(AssignedWorkoutDefinition definition)
    {
        await _navigation.GoTo("EditWorkoutDefinition", new Dictionary<string, object>
        {
            { nameof(EditWorkoutDefinitionViewModel.WorkoutDefinition), definition.Definition }
        });
    }
    
    [RelayCommand]
    public async Task Save()
    {
        _trackerDb.UpdateViewModel(_workoutProgram);
        await Back();
    }
    
    [RelayCommand]
    public async Task Back()
    {
        _workoutProgram = null;
        await _navigation.GoBack();
    }

    public void Refresh()
    {
        // Force update to refresh form
        WorkoutProgram = JsonSerializer.Deserialize<WorkoutProgram>(JsonSerializer.Serialize(_workoutProgram));
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (_workoutProgram is not null)
        {
            return;
        }

        if (query.TryGetValue(nameof(WorkoutProgram), out object value))
        {
            WorkoutProgram = value as WorkoutProgram;
        }
    }
}
