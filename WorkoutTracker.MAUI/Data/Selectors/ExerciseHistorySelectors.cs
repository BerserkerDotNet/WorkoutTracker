using System.Collections.Generic;
using System.Linq;

namespace WorkoutTracker.MAUI.Data.Selectors;

public static class ExerciseHistorySelectors
{

    public static Dictionary<DateOnly, IEnumerable<LogEntryViewModel>> SelectHistory(RootState state)
    {
        return state.ExerciseLogs?.History ?? new Dictionary<DateOnly, IEnumerable<LogEntryViewModel>>();
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

    public static IEnumerable<LogEntryViewModel> SelectTodayExercises(RootState state)
    {
        var history = SelectHistory(state);

        var today = DateOnly.FromDateTime(DateTime.Today.ToUniversalTime());
        return history.ContainsKey(today) ? history[today] : Enumerable.Empty<LogEntryViewModel>();
    }

    public static LogEntryViewModel SelectTodayExerciseById(RootState state, Guid id)
    {
        return SelectTodayExercises(state).FirstOrDefault(e => e.Exercise.Id == id);
    }

    public static PreviousLogRecordStats SelectLastLogByExercise(RootState state, Guid id)
    {
        var lookup = SelectLastLogByExerciseLookup(state);
        var record = lookup.ContainsKey(id) ? lookup[id] : null;
        if (record is null) 
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
