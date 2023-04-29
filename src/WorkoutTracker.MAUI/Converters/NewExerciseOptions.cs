using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.MAUI.Converters
{
    public record NewExerciseOptions(ExerciseSelectorType ExerciseSelector, ProgressiveOverloadType OverloadType);
}