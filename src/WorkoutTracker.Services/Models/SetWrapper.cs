using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Extensions;

namespace WorkoutTracker.Services.Models;

public record SetWrapper(int Number, IExerciseSet Set, LogEntryViewModel Model)
{
    public Color Color => Set.GetColor();
}