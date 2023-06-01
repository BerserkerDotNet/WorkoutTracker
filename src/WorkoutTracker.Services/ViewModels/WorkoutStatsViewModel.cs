using CommunityToolkit.Mvvm.ComponentModel;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.Services.ViewModels;

public sealed partial class WorkoutStatsViewModel : ObservableObject
{
    private readonly IWorkoutDataProvider _trackerDb;

    [ObservableProperty]
    private TotalWorkoutData _totalWorkoutData;
    
    public WorkoutStatsViewModel(IWorkoutDataProvider trackerDb)
    {
        _trackerDb = trackerDb;
    }

    public void Init()
    {
        TotalWorkoutData = _trackerDb.GetWorkoutStatistics();
    }
}