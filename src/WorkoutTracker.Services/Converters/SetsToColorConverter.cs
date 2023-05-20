using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Services.Converters
{
    public sealed class SetsToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<IExerciseSet> sets)
            {
                if (sets.OfType<ProposedSet>().Count() == sets.Count())
                {
                    return Colors.Transparent;
                }

                return sets.OfType<CompletedSet>().Count() == sets.Count() ? Colors.LightGreen : Colors.LightSkyBlue;
            }

            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}