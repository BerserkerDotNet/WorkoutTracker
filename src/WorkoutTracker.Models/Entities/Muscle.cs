using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.Models.Entities
{
    [PluralName(EndpointNames.MusclePluralName)]
    public class Muscle : EntityBase
    {
        public string Name { get; set; }

        public string MuscleGroup { get; set; }

        public string ImagePath { get; set; }

        public static Muscle FromViewModel(MuscleViewModel vm)
        {
            return new Muscle
            {
                Id = vm.Id,
                Name = vm.Name,
                ImagePath = vm.ImagePath,
                MuscleGroup = vm.MuscleGroup,
            };
        }

        public MuscleViewModel ToViewModel()
        {
            return new MuscleViewModel
            {
                Id = Id,
                Name = Name,
                ImagePath = ImagePath,
                MuscleGroup = MuscleGroup,
            };
        }
    }
}
