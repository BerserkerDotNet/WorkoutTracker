using System.Globalization;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Services.Converters;

public sealed class SetToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value switch
        {
            CompletedSet cSet => Colors.LightGreen,
            InProgressSet iSet => Colors.LightPink,
            LegacySet lSet => Colors.LightGreen,
            ProposedSet => Colors.LightSkyBlue,
            _ => Colors.Transparent
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}