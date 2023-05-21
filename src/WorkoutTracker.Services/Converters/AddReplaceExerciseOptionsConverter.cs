using System.Globalization;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.Services.Converters
{
    public sealed class AddReplaceExerciseOptionsConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2)
            {
                return null;
            }

            if (values[0] is ExerciseViewModel exercise && values[1] is bool includeWarmup)
            {
                return new AddReplaceExerciseOptions(exercise, includeWarmup);
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}