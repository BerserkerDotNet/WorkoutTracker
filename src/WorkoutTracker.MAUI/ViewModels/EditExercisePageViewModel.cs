using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Services;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services;

namespace WorkoutTracker.MAUI.ViewModels;

[QueryProperty(nameof(Exercise), nameof(Exercise))]
public sealed partial class EditExercisePageViewModel : ObservableObject
{
    private readonly WorkoutTrackerDb _repository;
    private readonly ApplicationContext<EditExercisePageViewModel> _context;

    [ObservableProperty]
    private ExerciseViewModel _exercise;

    [ObservableProperty]
    private ObservableCollection<MuscleViewModel> _muscles;

    public EditExercisePageViewModel(WorkoutTrackerDb repository, ApplicationContext<EditExercisePageViewModel> context)
    {
        _repository = repository;
        _context = context;
    }

    public bool IsInitialized { get; private set; }

    [RelayCommand]
    public void AddTag(string text)
    {
        _exercise.Tags.Add(text);
    }

    [RelayCommand]
    public void LoadMuscles()
    {
        var muscles = _repository.GetMuscles();
        Muscles = new ObservableCollection<MuscleViewModel>(muscles);
        IsInitialized = true;
    }

    [RelayCommand]
    public async Task SaveExercise()
    {
        try
        {
            _repository.UpdateViewModel(_exercise);
            _context.ShowToast("Exercise saved!");
            await Shell.Current.GoToAsync("..");
        }
        catch (System.Exception ex)
        {
            _context.ShowError($"Something went wrong saving the exercise! {ex.Message}");
        }
    }
}