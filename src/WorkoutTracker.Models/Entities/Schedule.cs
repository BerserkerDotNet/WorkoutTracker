using System;

namespace WorkoutTracker.Models.Entities
{
    public class Schedule
    {
        public required WorkoutDefinition Monday { get; set; }

        public required WorkoutDefinition Tuesday { get; set; }

        public required WorkoutDefinition Wednesday { get; set; }

        public required WorkoutDefinition Thursday { get; set; }

        public required WorkoutDefinition Friday { get; set; }

        public required WorkoutDefinition Saturday { get; set; }

        public required WorkoutDefinition Sunday { get; set; }

        public WorkoutDefinition From(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => Monday,
                DayOfWeek.Tuesday => Tuesday,
                DayOfWeek.Wednesday => Wednesday,
                DayOfWeek.Thursday => Thursday,
                DayOfWeek.Friday => Friday,
                DayOfWeek.Saturday => Saturday,
                DayOfWeek.Sunday => Sunday,
                _ => WorkoutDefinition.Rest
            };
        }

        public static Schedule Default => new Schedule
        {
            Monday = WorkoutDefinition.Rest,
            Tuesday = WorkoutDefinition.Rest,
            Wednesday = WorkoutDefinition.Rest,
            Thursday = WorkoutDefinition.Rest,
            Friday = WorkoutDefinition.Rest,
            Saturday = WorkoutDefinition.Rest,
            Sunday = WorkoutDefinition.Rest
        };
    }
}