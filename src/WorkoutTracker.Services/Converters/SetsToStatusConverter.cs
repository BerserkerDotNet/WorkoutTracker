using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Services.Converters
{
    public sealed class SetsToStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<IExerciseSet> sets)
            {
                return $"{sets.OfType<CompletedSet>().Count()}/{sets.Count()}";
            }

            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}