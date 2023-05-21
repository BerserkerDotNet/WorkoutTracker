using Microsoft.Maui.Graphics;
using System;
using System.Globalization;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Services.Converters
{
    public sealed class TimerModeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ExerciseTimerMode mode)
            {
                return mode == ExerciseTimerMode.Resting ? Color.FromArgb("#2088ff") : Colors.IndianRed;
            }

            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}