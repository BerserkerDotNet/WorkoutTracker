using System;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.MAUI.Controls.WorkoutDefinition;

public partial class PowerLadderEditor : ContentView
{
    public PowerLadderEditor(PowerLadderOverloadFactor factor)
    {
        InitializeComponent();
        BindingContext = factor;
    }
}