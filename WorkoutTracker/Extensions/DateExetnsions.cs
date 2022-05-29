namespace WorkoutTracker;

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
