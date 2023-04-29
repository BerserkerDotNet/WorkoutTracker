using System;
using System.Globalization;

namespace WorkoutTracker.MAUI.Converters
{
    public sealed class SelectedWorkoutColor : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var workout = values[0] as Guid?;
            var currentWorkout = values[1] as Guid?;

            if (workout is not null &&  workout == currentWorkout)
            {
                return "selected";
            }

            return "notselected";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}