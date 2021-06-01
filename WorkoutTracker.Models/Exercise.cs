namespace WorkoutTracker.Models
{
    public class Exercise : EntityBase
    {
        public string Name { get; set; }

        public byte[] Icon { get; set; }

        public string Muscles { get; set; }

        public string Tags { get; set; }
    }
}
