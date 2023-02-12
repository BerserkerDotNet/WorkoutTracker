using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WorkoutTracker.MAUI.Interfaces;
using WorkoutTracker.MAUI.Services;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.MAUI.Converters
{
    public class ImagePathConverter : IValueConverter
    {
        CDNImageProvider _provider;

        public ImagePathConverter()
        {
            _provider = App.Current.ServiceProvider.GetRequiredService<CDNImageProvider>();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _provider.GetFullPath(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class TimerModeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ExerciseTimerMode mode)
            {
                return mode == ExerciseTimerMode.Resting ? Color.FromHex("#2088ff") : Colors.IndianRed;
            }

            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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