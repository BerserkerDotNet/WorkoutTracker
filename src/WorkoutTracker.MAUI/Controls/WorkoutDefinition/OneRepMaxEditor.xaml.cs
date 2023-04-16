using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.MAUI.Controls.WorkoutDefinition
{
    public partial class OneRepMaxEditor : ContentView
    {
        public OneRepMaxEditor(OneRepMaxProgressiveOverloadFactor oneRepMaxProgressiveOverloadFactor)
        {
            InitializeComponent();
            BindingContext = oneRepMaxProgressiveOverloadFactor;
        }
    }
}