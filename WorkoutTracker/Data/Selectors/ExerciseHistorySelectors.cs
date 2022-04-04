using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.Data.Selectors;

public static class ExerciseHistorySelectors
{
    public static Dictionary<DateOnly, IEnumerable<LogEntryViewModel>> SelectHistory(RootState state)
    {
        return state.ExerciseLogs?.History ?? new Dictionary<DateOnly, IEnumerable<LogEntryViewModel>>();
    }

    public static DateOnly SelectDate(RootState state)
    {
        var uiState = state.ExerciseLogs?.UIState;
        if (uiState is null) 
        {
            return DateOnly.FromDateTime(DateTime.Today.ToUniversalTime());
        }

        return uiState.SelectedDate;
    }

    public static Dictionary<Guid, LogEntryViewModel> SelectLastLogByExerciseLookup(RootState state)
    {
        return state.ExerciseLogs?.LastLogByExercise ?? new Dictionary<Guid, LogEntryViewModel>();
    }

    public static Dictionary<Guid, int> SelectTodayExerciseCountLookup(RootState state)
    {
        var history = SelectHistory(state);

        var today = DateOnly.FromDateTime(DateTime.Today.ToUniversalTime());
        return history.ContainsKey(today) ? history[today].ToDictionary(k => k.Exercise.Id, v => v.Sets.Count()) : new Dictionary<Guid, int>();
    }

    public static IEnumerable<LogEntryViewModel> SelectExercisesByDate(RootState state, DateTime date)
    {
        var history = SelectHistory(state);
        var key = DateOnly.FromDateTime(date);
        return history.ContainsKey(key) ? history[key] : Enumerable.Empty<LogEntryViewModel>();
    }

    public static IEnumerable<LogEntryViewModel> SelectTodayExercises(RootState state)
    {
        return SelectExercisesByDate(state, DateTime.Today.ToUniversalTime());
    }

    public static LogEntryViewModel SelectTodayExerciseById(RootState state, Guid id)
    {
        return SelectTodayExercises(state).FirstOrDefault(e => e.Exercise.Id == id);
    }

    public static LogEntryViewModel SelectExerciseLog(RootState state, DateTime date, Guid id)
    {
        return SelectExercisesByDate(state, date).FirstOrDefault(e => e.Id == id);
    }

    public static PreviousLogRecordStats SelectLastLogByExercise(RootState state, Guid id)
    {
        var lookup = SelectLastLogByExerciseLookup(state);
        var record = lookup.ContainsKey(id) ? lookup[id] : null;
        if (record is null || !record.Sets.Any()) 
        {
            return null;
        }

        var maxWeightSet = record.Sets.MaxBy(s => s.Weight);

        return new PreviousLogRecordStats(maxWeightSet.Weight, maxWeightSet.Repetitions);
    }

    public static bool IsLastLogByExerciseLoaded(RootState state, Guid id)
    {
        var lookup = SelectLastLogByExerciseLookup(state);
        return lookup.ContainsKey(id);
    }
}
