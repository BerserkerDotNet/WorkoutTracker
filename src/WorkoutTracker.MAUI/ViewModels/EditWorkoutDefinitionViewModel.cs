﻿using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Converters;
using WorkoutTracker.MAUI.Services.Data;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Models.Selectors;

namespace WorkoutTracker.MAUI.ViewModels;

public sealed partial class EditWorkoutDefinitionViewModel : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    private WorkoutDefinition _workoutDefinition;

    [ObservableProperty]
    private ObservableCollection<ExerciseViewModel> _exercises;
    
    [ObservableProperty]
    private ObservableCollection<MuscleViewModel> _muscles;

    [ObservableProperty]
    private bool _isNewExerciseMenuVisible;

    [ObservableProperty]
    private  ChipOption<ExerciseSelectorType>[] _exerciseSelectorTypes;
    
    [ObservableProperty]
    private  ChipOption<ProgressiveOverloadType>[] _overloadSelectorTypes;

    private readonly WorkoutTrackerDb _workoutTrackerDb;

    public EditWorkoutDefinitionViewModel(WorkoutTrackerDb workoutTrackerDb)
    {
        _workoutTrackerDb = workoutTrackerDb;
    }

    public void Init()
    {
        Exercises = new ObservableCollection<ExerciseViewModel>(_workoutTrackerDb.GetExercises());
        Muscles = new ObservableCollection<MuscleViewModel>(_workoutTrackerDb.GetMuscles());
        
        ExerciseSelectorTypes = new ChipOption<ExerciseSelectorType>[] {
            new ("Specific exercise", ExerciseSelectorType.SpecificExercise),
            new ("Specific muscle", ExerciseSelectorType.SpecificMuscle),
            new ("Muscle group", ExerciseSelectorType.MuscleGroup),
        };
        
        OverloadSelectorTypes = new ChipOption<ProgressiveOverloadType>[]{
            new ("Power ladder", ProgressiveOverloadType.PowerLadder),
            new ("Reps ladder", ProgressiveOverloadType.RepsLadder),
            new ("One rep max %", ProgressiveOverloadType.OneRepMaxPercentage),
            new ("Steady state", ProgressiveOverloadType.SteadyState),
        };        
    }

    [RelayCommand]
    public void AddExercise(NewExerciseOptions options)
    {
        WorkoutDefinition.Exercises.Add(new ExerciseDefinition
        {
            ExerciseSelector = options.ExerciseSelector switch
            {
                ExerciseSelectorType.SpecificExercise => new SpecificExerciseSelector(Guid.NewGuid()),
                ExerciseSelectorType.MuscleGroup => new MuscleGroupExerciseSelector("Back"),
                ExerciseSelectorType.SpecificMuscle => new MuscleExerciseSelector(Guid.NewGuid()),
                _ => new MuscleGroupExerciseSelector("Back")
            },
            OverloadFactor = options.OverloadType switch
            {
                ProgressiveOverloadType.PowerLadder => new PowerLadderOverloadFactor(5, true),
                ProgressiveOverloadType.OneRepMaxPercentage => new OneRepMaxProgressiveOverloadFactor(80, 3),
                ProgressiveOverloadType.SteadyState => new SteadyStateProgressiveOverloadFactor(0, 3, 10),
                ProgressiveOverloadType.RepsLadder => new RepetitionsLadderOverloadFactor(1, true),
                _ => new SteadyStateProgressiveOverloadFactor(0, 3, 10),
            }
        });
        IsNewExerciseMenuVisible = false;
    }

    [RelayCommand]
    public async Task Save()
    {
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public void ShowNewExerciseMenu()
    {
        IsNewExerciseMenuVisible = true;
    }
    
    [RelayCommand]
    public void HideNewExerciseMenu()
    {
        IsNewExerciseMenuVisible = false;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey(nameof(WorkoutDefinition)))
        {
            var definition = query[nameof(WorkoutDefinition)] as WorkoutDefinition;
            definition.Exercises = new ObservableCollection<ExerciseDefinition>(definition.Exercises);
            WorkoutDefinition = definition;
        }
    }
}