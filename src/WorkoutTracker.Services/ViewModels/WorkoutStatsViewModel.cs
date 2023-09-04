using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.Services.ViewModels;

public sealed partial class WorkoutStatsViewModel : ObservableObject
{
    private readonly IWorkoutDataProvider _trackerDb;

    [ObservableProperty]
    private WorkoutsSummary _workoutsSummary;
    
    [ObservableProperty]
    private WorkoutTimeMetrics _timeMetrics;
    
    [ObservableProperty]
    private ObservableCollection<DataSeriesItem> _muscleGroupStats;
    
    [ObservableProperty]
    private ObservableCollection<LogEntryViewModel> _workoutHistory;

    [ObservableProperty]
    private bool isRefreshing;
    
    public WorkoutStatsViewModel(IWorkoutDataProvider trackerDb)
    {
        _trackerDb = trackerDb;
    }

    public void Init()
    {
        var stats = _trackerDb.GetWorkoutStatistics();
        var logs = _trackerDb.GetWorkoutLogs(DateTime.Today, 7);

        WorkoutsSummary = stats.Summary;
        TimeMetrics = stats.TimeMetrics;
        MuscleGroupStats = new ObservableCollection<DataSeriesItem>(stats.PercentagePerMuscleGroup);
        WorkoutHistory = new ObservableCollection<LogEntryViewModel>(logs);
    }

    [RelayCommand]
    public void LoadMoreLogs()
    {
        var minDate = WorkoutHistory.Min(w => w.Date).AddDays(-1);
        var logs = _trackerDb.GetWorkoutLogs(minDate, 7);
        foreach (var log in logs)
        {
            WorkoutHistory.Add(log);
        }

        IsRefreshing = false;
    }
}