namespace WorkoutTracker.Data.Selectors;

public static class ScheduleSelectors
{
    public static IEnumerable<WorkoutViewModel> SelectSchedule(this RootState state)
    {
        return state?.ExerciseSchedule?.WorkoutSchedule ?? Enumerable.Empty<WorkoutViewModel>();
    }

    public static WorkoutViewModel SelectScheduleById(this RootState state, Guid id)
    {
        return SelectSchedule(state).FirstOrDefault(s => s.Id == id);
    }

    public static WorkoutViewModel SelectNextExerciseFromSchedule(this RootState state, Guid currentScheduleId)
    {
        return SelectSchedule(state)
            .SkipWhile(s => s.Id != currentScheduleId)
            .Take(2)
            .Last();
    }

    public static WorkoutViewModel SelectPreviousExerciseFromSchedule(this RootState state, Guid currentScheduleId)
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
