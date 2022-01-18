namespace WorkoutTracker.Models
{
    [PluralName("Muscles")]
    public class Muscle : EntityBase
    {
        public string Name { get; set; }

        public string MuscleGroup { get; set; }

        public string Image { get; set; }

        public byte[] ImageRaw { get; set; }
    }
}
