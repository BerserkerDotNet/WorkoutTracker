using AndroidX.Work;
using CommunityToolkit.Mvvm.Input;
using DevExpress.Data.Filtering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.MAUI.Views;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.MAUI.ViewModels;

public sealed partial class LibraryViewModel : ObservableObject
{
    private readonly WorkoutTrackerDb _repository;

    [ObservableProperty]
    private IEnumerable<ExerciseViewModel> _exercises;

    [ObservableProperty]
    private bool _isRefreshingData;

    [ObservableProperty]
    private bool _isFilterVisible;

    [ObservableProperty]
    private string[] _muscleGroups;

    [ObservableProperty]
    private CriteriaOperator _currentFilter;

    public LibraryViewModel(WorkoutTrackerDb repository)
    {
        _repository = repository;
    }

    public bool IsInitialized { get; private set; }

    [RelayCommand]
    public void LoadExercises()
    {
        IsRefreshingData = true;
        Task.Run(() =>
        {
            Exercises = _repository.GetExercises();
            Application.Current.Dispatcher.Dispatch(() =>
            {
                MuscleGroups = Exercises.SelectMany(e => e.MuscleGroups).Distinct().ToArray();
                IsRefreshingData = false;
                IsInitialized = true;
            });
        });
    }

    [RelayCommand]
    public void ShowFilter()
    {
        IsFilterVisible = true;
    }

    [RelayCommand]
    public void HideFilter()
    {
        IsFilterVisible = false;
    }

    [RelayCommand]
    public void ApplyFilter(IList groups)
    {
        HideFilter();

        if (groups.Count == 0)
        {
            CurrentFilter = null;
        }
        else
        {
            CurrentFilter = CriteriaOperator.FromLambda<ExerciseViewModel>(e => e.MuscleGroups.Any(g => groups.Contains(g)));
        }
    }

    [RelayCommand]
    public void SyncData()
    {
        using var builder = new Constraints.Builder();
        builder.SetRequiredNetworkType(NetworkType.Connected);
        var workConstraints = builder.Build();

        var workerRequest = new OneTimeWorkRequest.Builder(typeof(DataSyncWorker))
            .SetConstraints(workConstraints)
            .AddTag(DataSyncWorker.TAG)
            .Build();

        WorkManager.GetInstance(Microsoft.Maui.ApplicationModel.Platform.AppContext)
            .EnqueueUniqueWork(DataSyncWorker.TAG, ExistingWorkPolicy.Replace, workerRequest);
    }

    [RelayCommand]
    public async Task EditExercise(ExerciseViewModel exercise)
    {
        await Shell.Current.GoToAsync(nameof(EditExercisePage), new Dictionary<string, object>
        {
            { nameof(EditExercisePageViewModel.Exercise), exercise }
        });
    }
}