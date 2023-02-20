using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.MAUI.Services.Data.Entities;

public class MuscleDbEntity : BaseDbEntity
{
    public string Name { get; set; }

    public string MuscleGroup { get; set; }

    public string ImagePath { get; set; }

    public MuscleViewModel ToViewModel()
    {
        return new MuscleViewModel
        {
            Id = Id,
            Name = Name,
            MuscleGroup = MuscleGroup,
            ImagePath = ImagePath
        };
    }

    public static MuscleDbEntity FromViewModel(MuscleViewModel model)
    {
        return new MuscleDbEntity
        {
            Id = model.Id,
            Name = model.Name,
            MuscleGroup = model.MuscleGroup,
            ImagePath = model.ImagePath
        };
    }
}
