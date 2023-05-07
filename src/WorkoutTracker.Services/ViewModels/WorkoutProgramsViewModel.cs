using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Services.Interfaces;
using INavigation = WorkoutTracker.Services.Interfaces.INavigation;

namespace WorkoutTracker.Services.ViewModels;

public sealed partial class WorkoutProgramsViewModel : ObservableObject
{
    private readonly IWorkoutDataProvider _trackerDb;
    private readonly INavigation _navigation;

    [ObservableProperty]
    private ObservableCollection<WorkoutProgram> _programs;

    [ObservableProperty]
    private bool _isLoadingData;

    [ObservableProperty]
    private Guid? _selectedProgram;

    public WorkoutProgramsViewModel(IWorkoutDataProvider trackerDb, INavigation navigation)
    {
        _trackerDb = trackerDb;
        _navigation = navigation;
    }

    [RelayCommand]
    public void LoadPrograms()
    {
        IsLoadingData = true;
        Programs = new ObservableCollection<WorkoutProgram>(_trackerDb.GetPrograms());
        SelectedProgram = _trackerDb.GetProfile().CurrentWorkout;
        IsLoadingData = false;
    }

    [RelayCommand]
    public async Task EditProgram(WorkoutProgram program)
    {
        program ??= new WorkoutProgram { Name = "New program", Id = Guid.NewGuid(), Schedule = Schedule.Default };
        await _navigation.GoTo("EditWorkoutProgram", new Dictionary<string, object>
        {
            { nameof(EditWorkoutProgramViewModel.WorkoutProgram), program }
        });
    }

    [RelayCommand]
    public void DeleteProgram(WorkoutProgram program)
    {
        Programs.Remove(program);
        _trackerDb.DeleteViewModel(program);
    }

    [RelayCommand]
    public void SetCurrentWorkout(WorkoutProgram program)
    {
        if (SelectedProgram == program.Id)
        {
            return;
        }

        _trackerDb.SetCurrentWorkout(program.Id);
        SelectedProgram = program.Id;
    }
}
