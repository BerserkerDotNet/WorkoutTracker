namespace WorkoutTracker.MAUI.Services.Data.Entities
{
    public class WorkoutStatistics : BaseDbEntity
    {
        public int TotalWorkouts { get; set; }

        public int WorkoutsThisWeek { get; set; }
    
        public int WorkoutsThisMonth { get; set; }
    }
}