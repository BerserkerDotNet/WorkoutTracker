namespace WorkoutTracker.Data.Selectors;

public static class ExerciseHistorySelectors
{
    public static Dictionary<DateOnly, IEnumerable<LogEntryViewModel>> SelectHistory(this RootState state)
    {
        return state.ExerciseLogs?.History ?? new Dictionary<DateOnly, IEnumerable<LogEntryViewModel>>();
    }

    public static IEnumerable<WorkoutSummary> SelectSummaries(this RootState state)
    {
        return state.ExerciseLogs?.Summaries ?? Enumerable.Empty<WorkoutSummary>();
    }

    public static DateOnly SelectDate(this RootState state)
    {
        var date = state.ExerciseLogs?.SelectedDate;
        if (date is null)
        {
            return DateOnly.FromDateTime(DateTime.Today.ToUniversalTime());
        }

        return date.Value;
    }

    public static Dictionary<Guid, LogEntryViewModel> SelectLastLogByExerciseLookup(RootState state)
    {
        return state.ExerciseLogs?.LastLogByExercise ?? new Dictionary<Guid, LogEntryViewModel>();
    }

    public static Dictionary<Guid, int> SelectTodayExerciseCountLookup(this RootState state)
    {
        var history = SelectHistory(state);

        var today = DateOnly.FromDateTime(DateTime.Today.ToUniversalTime());
        return history.ContainsKey(today) ? history[today].ToDictionary(k => k.Exercise.Id, v => v.Sets.Count()) : new Dictionary<Guid, int>();
    }

    public static Dictionary<Guid, LogEntryViewModel> SelectTodayExerciseLogLookup(this RootState state)
    {
        var history = SelectHistory(state);

        var today = DateOnly.FromDateTime(DateTime.Today.ToUniversalTime());
        return history.ContainsKey(today) ? history[today].ToDictionary(k => k.Exercise.Id, v => v) : new Dictionary<Guid, LogEntryViewModel>();
    }

    public static IEnumerable<LogEntryViewModel> SelectExercisesByDate(this RootState state, DateOnly date)
    {
        var history = SelectHistory(state);
        return history.ContainsKey(date) ? history[date] : Enumerable.Empty<LogEntryViewModel>();
    }

    public static IEnumerable<LogEntryViewModel> SelectTodayExercises(RootState state)
    {
        return SelectExercisesByDate(state, DateOnly.FromDateTime(DateTime.Today.ToUniversalTime()));
    }

    public static LogEntryViewModel SelectTodayExerciseById(this RootState state, Guid id)
    {
        return SelectTodayExercises(state).FirstOrDefault(e => e.Exercise.Id == id);
    }

    public static LogEntryViewModel SelectExerciseLog(this RootState state, DateOnly date, Guid id)
    {
        return SelectExercisesByDate(state, date).FirstOrDefault(e => e.Id == id);
    }

    public static Dictionary<Guid, PreviousLogRecordStats> SelectLastLogByExercise(this RootState state, IEnumerable<WorkoutViewModel> schedule)
    {
        return schedule.ToDictionary(k => k.Exercise.Id, v => state.SelectLastLogByExercise(v.Exercise.Id));
    }

    public static PreviousLogRecordStats SelectLastLogByExercise(this RootState state, Guid id)
    {
        var summaries = state.SelectSummaries()
            .Where(s => s.ExerciseId == id)
            .OrderByDescending(s => s.Date);

        var maxWeight = summaries.MaxBy(s => s.Max.WeightLb);
        if (maxWeight is null)
        {
            return null;
        }

        var showKG = state.SelectShowWeightInKG();
        var weight = showKG ? maxWeight.Max.WeightKg : maxWeight.Max.WeightLb;
        var weightUnit = showKG ? "KG" : "LB";

        return new PreviousLogRecordStats(maxWeight, summaries.First());
    }
}
