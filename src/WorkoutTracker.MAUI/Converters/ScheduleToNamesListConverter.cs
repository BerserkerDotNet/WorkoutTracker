using System;
using System.Globalization;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.MAUI.Converters
{
    public sealed class ScheduleToNamesListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Schedule schedule)
            {
                return new[]
                {
                    $"Mon: {schedule.Monday.Name}", $"Tue: {schedule.Tuesday.Name}", $"Wed: {schedule.Wednesday.Name}", $"Thu: {schedule.Thursday.Name}", $"Fri: {schedule.Friday.Name}",
                    $"Sat: {schedule.Saturday.Name}", $"Sun: {schedule.Sunday.Name}"
                };
            }

            return Array.Empty<WorkoutDefinition>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}