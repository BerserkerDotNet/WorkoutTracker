using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using WorkoutTracker.MAUI.Interfaces;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Models.Selectors;
using WorkoutTracker.Services.Extensions;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.Services.ViewModels;

public sealed partial class WorkoutViewModel : ObservableObject
{
    private readonly IWorkoutDataProvider _trackerDb;
    private readonly SetsGenerator _setsGenerator;
    private readonly IExerciseTimerService _timer;
    private readonly ApplicationContext<WorkoutViewModel> _context;

    [ObservableProperty]
    private ObservableCollection<LogEntryViewModel> _todaySets = new ObservableCollection<LogEntryViewModel>();

    [ObservableProperty]
    private TimeSpan _currentTime;

    [ObservableProperty]
    private ExerciseTimerMode _timerMode;

    [ObservableProperty]
    private bool _isSetEditorVisible;

    [ObservableProperty]
    private bool _isLoadingData;

    [ObservableProperty]
    private bool _isReplaceExerciseVisible;

    [ObservableProperty]
    private CompletedSet _completedSet;

    [ObservableProperty]
    private LogEntryViewModel _selectedModel;

    [ObservableProperty]
    private ObservableCollection<ExerciseViewModel> _exercises;

    public WorkoutViewModel(IWorkoutDataProvider trackerDb, SetsGenerator setsGenerator, IExerciseTimerService timer, ApplicationContext<WorkoutViewModel> context)
    {
        _trackerDb = trackerDb;
        _setsGenerator = setsGenerator;
        _timer = timer;
        _context = context;
        _timer.OnTick += OnTick; // use messaging
    }

    private void OnTick(object sender, TimerTickEventArgs e)
    {
        CurrentTime = e.CurrentTime;
        TimerMode = e.Mode;
    }

    [RelayCommand]
    public void LoadExercises()
    {
        Exercises = new ObservableCollection<ExerciseViewModel>(_trackerDb.GetExercises());
    }

    [RelayCommand]
    public void GetOrCreateWorkout()
    {
        IsLoadingData = true;
        Task.Run(() =>
        {
            var schedule = _trackerDb.GetTodaysSchedule().ToList();
            if (!schedule.Any())
            {
                var profile = _trackerDb.GetProfile();
                var program = WorkoutProgramProvider.Default;
                if (profile.CurrentWorkout.HasValue)
                {
                    var programs = _trackerDb.GetPrograms();
                    var selectedProgram = programs.FirstOrDefault(p => p.Id == profile.CurrentWorkout.Value);
                    if (selectedProgram is not null)
                    {
                        program = selectedProgram;
                    }
                }
                
                var todayWorkoutDefinition = program.Schedule.From(DateTime.Today.DayOfWeek);

                IList<ExerciseViewModel> exercises = Exercises;
                if (!exercises.Any())
                {
                    return;
                }

                var exerciseIdx = 0;
                var reservedExercises = todayWorkoutDefinition.Exercises
                .Where(e => e.ExerciseSelector is SpecificExerciseSelector)
                .Select(e => e.ExerciseSelector.Select(exercises).MatchedExercises.Single())
                .ToArray();
                // Remove reserved from the list

                foreach (var reservedExercise in reservedExercises)
                {
                    exercises.Remove(reservedExercise);
                }
                foreach (var exerciseDefinition in todayWorkoutDefinition.Exercises)
                {
                    // This sucks!
                    var exerciseDescriptor = exerciseDefinition.ExerciseSelector switch
                    {
                        SpecificExerciseSelector specific => specific.Select(reservedExercises),
                        IExerciseSelector other => other.Select(exercises)
                    };

                    var index = Random.Shared.Next(0, exerciseDescriptor.MatchedExercises.Count());
                    var exercise = exerciseDescriptor.MatchedExercises.ElementAt(index);

                    var sets = _setsGenerator.Generate(exercise.Id, exerciseDefinition.OverloadFactor).OfType<IExerciseSet>();
                    var log = new LogEntryViewModel
                    {
                        Date = DateTime.Today.ToUniversalTime(),
                        Id = Guid.NewGuid(),
                        Order = exerciseIdx,
                        Exercise = exercise,
                        Sets = new ObservableCollection<IExerciseSet>(sets)
                    };

                    schedule.Add(log);
                    _trackerDb.UpdateViewModel(log);

                    exerciseIdx++;
                    exercises.Remove(exercise);
                }
            }

            Application.Current.Dispatcher.Dispatch(() =>
            {
                IsLoadingData = false;
                TodaySets = new ObservableCollection<LogEntryViewModel>(schedule);
            });
        });
    }

    [RelayCommand]
    public void StartTimer()
    {
        if (!_timer.IsRunning)
        {
            _timer.Start();
        }
    }

    [RelayCommand]
    public void SaveSet()
    {
        _trackerDb.UpdateViewModel(SelectedModel);
        IsSetEditorVisible = false;

        var sets = SelectedModel.Sets as ObservableCollection<IExerciseSet>;
        var idx = sets.IndexOf(CompletedSet);
        sets[idx] = CompletedSet;

        SelectedModel = null;
        CompletedSet = null;

        _context.ShowToast("Entry saved.");
    }

    [RelayCommand]
    public void ShowReplaceDialog(LogEntryViewModel model)
    {
        SelectedModel = model;
        IsReplaceExerciseVisible = true;
    }

    [RelayCommand]
    public void AddSet(LogEntryViewModel model)
    {
        var sets = model.Sets as ObservableCollection<IExerciseSet>;
        sets.Add(new ProposedSet { Repetitions = 0, Weight = 0 });
        _trackerDb.UpdateViewModel(model);
    }

    [RelayCommand]
    public void ReduceSets(LogEntryViewModel model)
    {
        var sets = model.Sets as ObservableCollection<IExerciseSet>;
        if (sets.Last() is ProposedSet)
        {
            sets.Remove(sets.Last());
            _trackerDb.UpdateViewModel(model);
        }
    }

    [RelayCommand]
    public void RunSetOperation(SetWrapper wrapper)
    {
        SelectedModel = wrapper.Model;
        var status = wrapper.Set.GetStatus();
        if (status == SetStatus.NotStarted)
        {
            var sets = wrapper.Model.Sets as ObservableCollection<IExerciseSet>;
            var idx = wrapper.Number - 1;
            sets[idx] = new InProgressSet { Repetitions = wrapper.Set.Repetitions, Weight = wrapper.Set.Weight, RestTime = _currentTime };

            _timer.SetMode(ExerciseTimerMode.Exercising);
        }
        else if (status == SetStatus.InProgress)
        {
            var sets = wrapper.Model.Sets as ObservableCollection<IExerciseSet>;
            var idx = wrapper.Number - 1;
            var inProgressSet = wrapper.Set as InProgressSet;
            var completedSet = new CompletedSet { Repetitions = inProgressSet.Repetitions, Weight = inProgressSet.Weight, RestTime = inProgressSet.RestTime, Duration = _currentTime, CompletionTime = DateTime.UtcNow };
            sets[idx] = completedSet;
            CompletedSet = completedSet;

            _timer.SetMode(ExerciseTimerMode.Resting);
            IsSetEditorVisible = true;
        }
        else if (status == SetStatus.Completed)
        {
            var completedSet = wrapper.Set as CompletedSet;
            CompletedSet = completedSet;

            IsSetEditorVisible = true;
        }
    }

    [RelayCommand]
    public void ReplaceExercise(ExerciseViewModel exercise)
    {
        var sets = _setsGenerator.Generate(exercise.Id, new PowerLadderOverloadFactor(5, includeWarmup: true)).OfType<IExerciseSet>();

        if (SelectedModel is null) // Add
        {
            var newLogEntry = new LogEntryViewModel
            {
                Id = Guid.NewGuid(),
                Date = DateTime.Today.ToUniversalTime(),
                Exercise = exercise,
                Order = TodaySets.Count - 1,
                Sets = new ObservableCollection<IExerciseSet>(sets)
            };
            TodaySets.Add(newLogEntry);
            _trackerDb.UpdateViewModel(newLogEntry);
        }
        else // Replace
        {
            var newLogEntry = new LogEntryViewModel
            {
                Id = SelectedModel.Id,
                Date = SelectedModel.Date,
                Exercise = exercise,
                Order = SelectedModel.Order,
                Sets = new ObservableCollection<IExerciseSet>(sets)
            };

            var idx = TodaySets.IndexOf(SelectedModel);
            TodaySets[idx] = newLogEntry;
            _trackerDb.UpdateViewModel(newLogEntry);
        }

        SelectedModel = null;
        IsReplaceExerciseVisible = false;
    }

    [RelayCommand]
    public void RemoveExercise(LogEntryViewModel model)
    {
        var idx = TodaySets.IndexOf(model);
        TodaySets.RemoveAt(idx);
        _trackerDb.DeleteViewModel(model);
    }

    [RelayCommand]
    public void CancelExercise()
    {
        SelectedModel = null;
        IsReplaceExerciseVisible = false;
    }

    [RelayCommand]
    public void ShowAddExerciseDialog()
    {
        IsReplaceExerciseVisible = true;
    }
}