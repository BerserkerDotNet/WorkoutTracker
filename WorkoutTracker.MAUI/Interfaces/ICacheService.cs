using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.ViewModels;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI
{
    public interface ICacheService
    {
        bool IsExercisesCached();

        Task<IEnumerable<ExerciseViewModel>> GetExercises();

        Task SaveExercises(IEnumerable<ExerciseViewModel> exercises);

        void ResetExercisesCache();
    }
}