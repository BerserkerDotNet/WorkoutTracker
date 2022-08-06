namespace WorkoutTracker.Data.Selectors;

public static class ScheduleSelectors
{
    public static IEnumerable<ScheduleViewModel> SelectSchedule(this RootState state)
    {
        return state?.ExerciseSchedule?.Schedule ?? Enumerable.Empty<ScheduleViewModel>();
    }

    public static ScheduleViewModel SelectScheduleById(this RootState state, Guid id)
    {
        return SelectSchedule(state).FirstOrDefault(s => s.Id == id);
    }

    public static Guid? SelectCurentScheduleId(this RootState state)
    {
        return state?.ExerciseSchedule?.CurrentScheduleId ?? null;
    }

    public static ScheduleViewModel SelectNextExerciseFromSchedule(this RootState state, Guid currentScheduleId)
    {
        return SelectSchedule(state)
            .SkipWhile(s => s.Id != currentScheduleId)
            .Take(2)
            .Last();
    }

    public static ScheduleViewModel SelectPreviousExerciseFromSchedule(this RootState state, Guid currentScheduleId)
    {
        return SelectSchedule(state)
            .TakeWhile(s => s.Id != currentScheduleId)
            .LastOrDefault();
    }

    public static ExerciseProfile SelectCurrentProfile(this RootState state)
    {
        return state?.ExerciseSchedule?.SelectedProfile ?? ExerciseProfile.GetDefaultProfile();
    }
}
