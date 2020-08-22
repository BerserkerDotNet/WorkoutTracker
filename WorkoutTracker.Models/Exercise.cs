namespace WorkoutTracker.Models
{
    public class Exercise : EntityBase
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public string[] Muscles { get; set; }
    }
}
