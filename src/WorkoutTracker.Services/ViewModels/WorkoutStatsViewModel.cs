using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.Services.ViewModels;

public sealed partial class WorkoutStatsViewModel : ObservableObject
{
    private readonly IWorkoutDataProvider _trackerDb;

    [ObservableProperty]
    private WorkoutsSummary _workoutsSummary;
    
    [ObservableProperty]
    private ObservableCollection<DataSeriesItem> _muscleGroupStats;
    
    public WorkoutStatsViewModel(IWorkoutDataProvider trackerDb)
    {
        _trackerDb = trackerDb;
    }

    public void Init()
    {
        var stats = _trackerDb.GetWorkoutStatistics();

        WorkoutsSummary = stats.Summary;
        MuscleGroupStats = new ObservableCollection<DataSeriesItem>(stats.PercentagePerMuscleGroup);
    }
}