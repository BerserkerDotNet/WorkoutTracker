using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WorkoutTracker
{
    public interface ICacheService
    {
        Task<bool> IsExercisesCached();

        Task<IEnumerable<ExerciseViewModel>> GetExercises();

        Task SaveExercises(IEnumerable<ExerciseViewModel> exercises);

        Task ResetExercisesCache();

        Task<AccessToken> GetToken();

        Task SaveToken(AccessToken token);
    }
}