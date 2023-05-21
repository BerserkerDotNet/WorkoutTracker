using System;
using System.Globalization;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.Services.Converters;

public sealed class NewExerciseOptionsConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var exerciseSelector = values[0] as Option<ExerciseSelectorType>;
        var overloadSelector = values[1] as Option<ProgressiveOverloadType>;

        if (exerciseSelector is null || overloadSelector is null)
        {
            return null;
        }

        return new NewExerciseOptions(exerciseSelector.Value, overloadSelector.Value);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}