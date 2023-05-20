using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Services.Interfaces;

public interface ISetsGenerator
{
    IEnumerable<IExerciseSet> Generate(Guid exerciseId, IProgressiveOverloadFactor overloadFactor);
}