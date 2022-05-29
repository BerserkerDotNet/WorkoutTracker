using WorkoutTracker.Data.Reducers;

namespace WorkoutTracker;

public class RootState
{
    public ExercisesState Exercises { get; set; }

    public LogEntriesState ExerciseLogs { get; set; }

    public ExerciseScheduleState ExerciseSchedule { get; set; }
}
