using Microsoft.Maui.Graphics;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.Services.Extensions
{
    public static class IExerciseSetExtensions
    {
        public static Color GetColor(this IExerciseSet set)
        {
            return set switch
            {
                LegacySet => Colors.LightGreen,
                CompletedSet => Colors.LightGreen,
                InProgressSet => Colors.LightPink,
                ProposedSet => Colors.LightSkyBlue,
                _ => Colors.Gray
            };
        }

        public static SetStatus GetStatus(this IExerciseSet set)
        {
            return set switch
            {
                LegacySet => SetStatus.Completed,
                CompletedSet => SetStatus.Completed,
                InProgressSet => SetStatus.InProgress,
                ProposedSet => SetStatus.NotStarted,
                _ => SetStatus.NotStarted
            };
        }
    }
}