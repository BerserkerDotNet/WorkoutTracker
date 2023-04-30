using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Services;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.MAUI.Views;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.MAUI.ViewModels;

public sealed partial class EditWorkoutProgramViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private WorkoutProgram _workoutProgram;

    private readonly WorkoutTrackerDb _trackerDb;
    private readonly ApplicationContext<WorkoutViewModel> _context;

    public EditWorkoutProgramViewModel(WorkoutTrackerDb trackerDb, ApplicationContext<WorkoutViewModel> context)
    {
        _trackerDb = trackerDb;
        _context = context;
    }
    
    [RelayCommand]
    public async Task EditDefinition(AssignedWorkoutDefinition definition)
    {
        await Shell.Current.GoToAsync(nameof(EditWorkoutDefinition), new Dictionary<string, object>
        {
            { nameof(EditWorkoutDefinitionViewModel.WorkoutDefinition), definition.Definition }
        });
    }
    
    [RelayCommand]
    public async Task Save()
    {
        _trackerDb.UpdateViewModel(_workoutProgram);
        await Shell.Current.GoToAsync("..");
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
