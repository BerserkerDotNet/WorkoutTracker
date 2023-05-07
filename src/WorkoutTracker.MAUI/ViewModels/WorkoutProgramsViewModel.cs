using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Services;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.MAUI.Views;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.MAUI.ViewModels;

public sealed partial class WorkoutProgramsViewModel : ObservableObject
{
    private readonly WorkoutTrackerDb _trackerDb;
    private readonly ApplicationContext<WorkoutViewModel> _context;

    [ObservableProperty]
    private ObservableCollection<WorkoutProgram> _programs;

    [ObservableProperty]
    private bool _isLoadingData;

    [ObservableProperty]
    private Guid? _selectedProgram;

    public WorkoutProgramsViewModel(WorkoutTrackerDb trackerDb, ApplicationContext<WorkoutViewModel> context)
    {
        _trackerDb = trackerDb;
        _context = context;
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
        await Shell.Current.GoToAsync(nameof(EditWorkoutProgram), new Dictionary<string, object>
        {
            { nameof(EditWorkoutProgramViewModel.WorkoutProgram), program }
        });
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
