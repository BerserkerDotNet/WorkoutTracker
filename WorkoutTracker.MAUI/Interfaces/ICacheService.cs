using System.Collections.Generic;
using System.Threading.Tasks;
using WorkoutTracker.Models;

namespace WorkoutTracker.MAUI
{
    public interface ICacheService
    {
        bool IsExercisesCached();

        Task<IEnumerable<Exercise>> GetExercises();

        Task SaveExercises(IEnumerable<Exercise> exercises);
    }
}