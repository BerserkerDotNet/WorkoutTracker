using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WorkoutTracker
{
    public interface ICacheService
    {
        bool IsExercisesCached();

        Task<IEnumerable<ExerciseViewModel>> GetExercises();

        Task SaveExercises(IEnumerable<ExerciseViewModel> exercises);

        void ResetExercisesCache();

        Task<AccessToken> GetToken();

        Task SaveToken(AccessToken token);
    }
}