using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.MAUI.Controls.WorkoutDefinition
{
    public partial class SteadyStateEditor : ContentView
    {
        public SteadyStateEditor(SteadyStateProgressiveOverloadFactor factor)
        {
            InitializeComponent();
            BindingContext = factor;
        }
    }
}