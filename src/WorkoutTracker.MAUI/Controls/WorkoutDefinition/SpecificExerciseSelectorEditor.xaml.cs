using DevExpress.Maui.Editors;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Models.Selectors;

namespace WorkoutTracker.MAUI.Controls.WorkoutDefinition;

public partial class SpecificExerciseSelectorEditor : ContentView
{
    private SpecificExerciseSelector _selector;

    public SpecificExerciseSelectorEditor(SpecificExerciseSelector se, IEnumerable<ExerciseViewModel> exercises)
    {
        InitializeComponent();
        BindingContext = se;
        _selector = se;
        _exercisePicker.ItemsSource = exercises;
    }
}