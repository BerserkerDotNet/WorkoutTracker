using System;
using System.Globalization;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Services.Converters
{
    public sealed class ScheduleToListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Schedule schedule)
            {
                return new[]
                {
                    new AssignedWorkoutDefinition(DayOfWeek.Monday, nameof(DayOfWeek.Monday), schedule.Monday),
                    new AssignedWorkoutDefinition(DayOfWeek.Tuesday, nameof(DayOfWeek.Tuesday), schedule.Tuesday),
                    new AssignedWorkoutDefinition(DayOfWeek.Wednesday, nameof(DayOfWeek.Wednesday), schedule.Wednesday),
                    new AssignedWorkoutDefinition(DayOfWeek.Thursday, nameof(DayOfWeek.Thursday), schedule.Thursday),
                    new AssignedWorkoutDefinition(DayOfWeek.Friday, nameof(DayOfWeek.Friday), schedule.Friday),
                    new AssignedWorkoutDefinition(DayOfWeek.Saturday, nameof(DayOfWeek.Saturday), schedule.Saturday),
                    new AssignedWorkoutDefinition(DayOfWeek.Sunday, nameof(DayOfWeek.Sunday), schedule.Sunday),
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