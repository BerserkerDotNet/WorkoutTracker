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
    public partial class MuscleGroupExerciseSelectorEditor : ContentView
    {


        public MuscleGroupExerciseSelectorEditor(MuscleGroupExerciseSelector muscleGroupExerciseSelector,
            IEnumerable<MuscleViewModel> muscleGroups)
        {
            InitializeComponent();
            BindingContext = muscleGroupExerciseSelector;
            muscleGroupPicker.ItemsSource = muscleGroups.Select(m => m.MuscleGroup).ToArray();
        }
    }
}