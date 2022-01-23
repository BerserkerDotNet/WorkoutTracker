namespace WorkoutTracker.Models
{
    [PluralName(EntityPluralNames.MusclePluralName)]
    public class Muscle : EntityBase
    {
        public string Name { get; set; }

        public string MuscleGroup { get; set; }

        public string ImagePath { get; set; }
    }
}
