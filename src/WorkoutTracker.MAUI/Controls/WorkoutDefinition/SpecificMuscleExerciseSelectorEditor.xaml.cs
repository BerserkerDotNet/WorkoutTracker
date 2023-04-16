using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Models.Selectors;

namespace WorkoutTracker.MAUI.Controls.WorkoutDefinition
{
    public partial class SpecificMuscleExerciseSelectorEditor : ContentView
    {
        public SpecificMuscleExerciseSelectorEditor(MuscleExerciseSelector selector, IEnumerable<MuscleViewModel> muscles)
        {
            InitializeComponent();
            BindingContext = selector;
            musclePicker.ItemsSource = muscles;
        }
    }
}