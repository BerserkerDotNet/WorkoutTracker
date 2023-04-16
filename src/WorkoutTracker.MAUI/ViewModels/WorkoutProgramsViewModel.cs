using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Services;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.MAUI.Views;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.MAUI.ViewModels;

public sealed partial class WorkoutProgramsViewModel : ObservableObject
{
    private readonly WorkoutTrackerDb _trackerDb;
    private readonly ApplicationContext<WorkoutViewModel> _context;

    [ObservableProperty]
    private ObservableCollection<WorkoutProgram> _programs;

    [ObservableProperty]
    private bool _isLoadingData;

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
        IsLoadingData = false;
    }

    [RelayCommand]
    public async Task EditProgram(WorkoutProgram program)
    {
        program = program ?? new WorkoutProgram { Name = "New program", Id = Guid.NewGuid(), Schedule = Schedule.Default };
        await Shell.Current.GoToAsync(nameof(EditWorkoutProgram), new Dictionary<string, object>
        {
            { nameof(EditWorkoutProgramViewModel.WorkoutProgram), program }
        });
    }
}
