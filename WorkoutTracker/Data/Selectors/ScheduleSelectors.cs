namespace WorkoutTracker.Data.Selectors;

public static class ScheduleSelectors
{
    public static IEnumerable<ScheduleViewModel> SelectSchedule(RootState state)
    {
        return state?.ExerciseSchedule?.Schedule ?? Enumerable.Empty<ScheduleViewModel>();
    }

    public static ScheduleViewModel SelectScheduleById(RootState state, Guid id)
    {
        return SelectSchedule(state).FirstOrDefault(s => s.Id == id);
    }

    public static ScheduleViewModel SelectNextExerciseFromSchedule(RootState state, Guid currentScheduleId) 
    {
        return SelectSchedule(state)
            .SkipWhile(s => s.Id != currentScheduleId)
            .Take(2)
            .Last();
    }

    public static ScheduleViewModel SelectPreviousExerciseFromSchedule(RootState state, Guid currentScheduleId)
    {
        return SelectSchedule(state)
            .TakeWhile(s => s.Id != currentScheduleId)
            .LastOrDefault();
    }
}
