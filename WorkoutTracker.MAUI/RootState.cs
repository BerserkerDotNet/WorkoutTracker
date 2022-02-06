using WorkoutTracker.MAUI.Data.Reducers;

namespace WorkoutTracker.MAUI
{
	public class RootState
    {
        public ExercisesState Exercises { get; set; }

        public LogEntriesState ExerciseLogs { get; set; }

        public ExerciseScheduleState ExerciseSchedule { get; set; }
    }

    public static class DateExetnsions 
    {
        public static DateOnly ToDateOnly(this DateTime date) 
        {
            return DateOnly.FromDateTime(date);
        }

        public static DateTime ToDateTime(this DateOnly date)
        {
            return date.ToDateTime(TimeOnly.MinValue);
        }
    }
}
