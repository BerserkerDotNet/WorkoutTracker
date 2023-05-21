using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WorkoutTracker.Services.Converters
{
    public sealed class CollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value is not IEnumerable<object>)
            {
                return true;
            }

            if (value is IEnumerable<object> collection)
            {
                return !collection.Any();
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}