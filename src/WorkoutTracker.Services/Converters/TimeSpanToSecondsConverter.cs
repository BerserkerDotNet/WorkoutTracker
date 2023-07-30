using System;
using System.Globalization;

namespace WorkoutTracker.Services.Converters;

public sealed class TimeSpanToSecondsConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan span)
        {
            return span.TotalSeconds;
        }

        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        return TimeSpan.Zero;
    }
}

public class TimeSpanToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TimeSpan span)
        {
            if (span.TotalDays >= 1)
            {
                return $"{Math.Round(span.TotalDays, 1)} day(s)";
            }
            
            if (span.TotalHours >= 1)
            {
                return $"{Math.Round(span.TotalHours, 1)} hour(s)";
            }
            
            if (span.TotalMinutes >= 1)
            {
                return $"{Math.Round(span.TotalMinutes, 1)} minute(s)";
            }

            if (span.TotalSeconds >= 1)
            {
                return $"{Math.Round(span.TotalSeconds, 1)} second(s)";
            }
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}