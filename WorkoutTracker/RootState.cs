using WorkoutTracker.Data.Reducers;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker;

public class RootState
{
    public ExercisesState Exercises { get; set; }

    public LogEntriesState ExerciseLogs { get; set; }

    public ExerciseScheduleState ExerciseSchedule { get; set; }

    public UserPreferencesState Preferences { get; set; }

    public UIState UI { get; set; }

    public WorkoutDataState Data { get; set; }
}

public record WorkoutDataState(IEnumerable<WorkoutProgram> WorkoutPrograms);