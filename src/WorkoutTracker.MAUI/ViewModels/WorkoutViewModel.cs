using CommunityToolkit.Mvvm.Input;
using DevExpress.Maui.Scheduler.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Controls;
using WorkoutTracker.MAUI.Extensions;
using WorkoutTracker.MAUI.Interfaces;
using WorkoutTracker.MAUI.Services;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Models.Selectors;
using WorkoutTracker.Services;

namespace WorkoutTracker.MAUI.ViewModels;

public sealed partial class WorkoutViewModel : ObservableObject
{
    private readonly WorkoutTrackerDb _trackerDb;
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

    public WorkoutViewModel(WorkoutTrackerDb trackerDb, SetsGenerator setsGenerator, IExerciseTimerService timer, ApplicationContext<WorkoutViewModel> context)
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

                reservedExercises.ForEach(r => exercises.Remove(r));
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
    }

    [RelayCommand]
    public void ReduceSets(LogEntryViewModel model)
    {
        var sets = model.Sets as ObservableCollection<IExerciseSet>;
        if (sets.Last() is ProposedSet)
        {
            sets.Remove(sets.Last());
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

public sealed class WorkoutProgramProvider
{
    private static readonly Guid BarbellRow = Guid.Parse("b1ce8082-2b1a-4956-b3de-e687e3e16902");
    private static readonly Guid LatPullDown = Guid.Parse("d6846bae-ccf8-4073-8825-e9fb28def637");
    private static readonly Guid BarbellCurl = Guid.Parse("93b66b46-74bc-484f-bc52-844d5facba69");
    private static readonly Guid Facepulls = Guid.Parse("d4355b45-ac08-4ed6-8401-6a7cf9d91491");

    public static WorkoutProgram Default = new WorkoutProgram
    {
        Id = Guid.Parse("5184f670-ad0d-4062-a936-6a16c9e14069"),
        Name = "Default",
        Schedule = new Schedule
        {
            Monday = PullWorkout(),
            Tuesday = LegsDay(),
            Wednesday = PushDay(),
            Thursday = LegsDay(),
            Friday = UpperBodyDay(),
            Saturday = WorkoutDefinition.Rest,
            Sunday = WorkoutDefinition.Rest,
        }
    };

    public static WorkoutDefinition PullWorkout()
    {
        return new WorkoutDefinition
        {
            Name = "Pull day",
            Exercises = new List<ExerciseDefinition>
            {
                new ExerciseDefinition
                {
                    ExerciseSelector = new SpecificExerciseSelector(BarbellRow),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true)
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new SpecificExerciseSelector(LatPullDown),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false)
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Back"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false)
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new SpecificExerciseSelector(BarbellCurl),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true)
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Arm"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Arm"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new SpecificExerciseSelector(Facepulls),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                }
            }
        };
    }

    public static WorkoutDefinition LegsDay()
    {
        return new WorkoutDefinition
        {
            Name = "Legs day",
            Exercises = new List<ExerciseDefinition>
            {
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Quads"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Glutes"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Hamstrings"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Quads"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Calves"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false, workingReps: 15),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new SpecificExerciseSelector(Facepulls),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                }
            }
        };
    }

    public static WorkoutDefinition PushDay()
    {
        return new WorkoutDefinition
        {
            Name = "Push day",
            Exercises = new List<ExerciseDefinition>
            {
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Chest"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Chest"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Shoulder"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Shoulder"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Triceps"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new SpecificExerciseSelector(Facepulls),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                }
            }
        };
    }

    public static WorkoutDefinition UpperBodyDay()
    {
        return new WorkoutDefinition
        {
            Name = "Upper body day",
            Exercises = new List<ExerciseDefinition>
            {
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Back"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true)
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Back"),
                    OverloadFactor = new OneRepMaxProgressiveOverloadFactor(80, 3)
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Chest"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: true),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Shoulder"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Arm"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Arm"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new MuscleGroupExerciseSelector("Triceps"),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                },
                new ExerciseDefinition
                {
                    ExerciseSelector = new SpecificExerciseSelector(Facepulls),
                    OverloadFactor = new PowerLadderOverloadFactor(5, includeWarmup: false),
                }
            }
        };
    }
}