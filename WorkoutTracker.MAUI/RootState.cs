using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Data.Reducers;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI
{
    public class RootState
    {
        public ExercisesState Exercises { get; set; }
    }
}
