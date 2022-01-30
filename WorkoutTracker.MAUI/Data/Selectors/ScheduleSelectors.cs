using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.MAUI.Data.Selectors;

public static class ScheduleSelectors
{
    public static Dictionary<string, ScheduleViewModel> SelectSchedule(RootState state)
    {
        return state?.ExerciseSchedule?.Schedule ?? new Dictionary<string, ScheduleViewModel>();
    }

    public static ScheduleViewModel SelectScheduleByCategory(RootState state, string category)
    {
        var schedule = SelectSchedule(state);
        return schedule.ContainsKey(category) ? schedule[category] : null;
    }

    public static IEnumerable<ExerciseWithCategoryViewModel> SelectCurrentExercisesFromSchedule(RootState state)
    {
        return SelectSchedule(state)
            .Select(s => new ExerciseWithCategoryViewModel(s.Key, s.Value.Exercises.ElementAt(s.Value.CurrentIndex)))
            .ToArray();
    }

    public static ScheduleViewModel SelectNextExerciseFromSchedule(RootState state, string category) 
    {
        return SelectSchedule(state)
            .SkipWhile(s => s.Key != category)
            .Take(2)
            .Last()
            .Value;
    }

    public static ScheduleViewModel SelectPreviousExerciseFromSchedule(RootState state, string category)
    {
        return SelectSchedule(state)
            .TakeWhile(s => s.Key != category)
            .LastOrDefault()
            .Value;
    }
}
