using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.MAUI.Services.Data.Entities;

public class ExerciseDbEntity : BaseDbEntity
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Steps { get; set; }

    public string TutorialUrl { get; set; }

    public string ImagePath { get; set; }

    [ManyToMany(typeof(MusclesToExercises), CascadeOperations = CascadeOperation.CascadeRead)]
    public List<MuscleDbEntity> Muscles { get; set; }

    [TextBlob(nameof(TagsBlobbed))]
    public IEnumerable<string> Tags { get; set; }

    public string TagsBlobbed { get; set; }

    public ExerciseViewModel ToViewModel()
    {
        return new ExerciseViewModel
        {
            Id = Id,
            Name = Name,
            Description = Description,
            Steps = Steps,
            TutorialUrl = TutorialUrl,
            ImagePath = ImagePath,
            Tags = new ObservableCollection<string>(Tags),
            Muscles = new ObservableCollection<MuscleViewModel>(Muscles.Select(m => m.ToViewModel())),
        };
    }

    public static ExerciseDbEntity FromViewModel(ExerciseViewModel model)
    {
        return new ExerciseDbEntity
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Steps = model.Steps,
            TutorialUrl = model.TutorialUrl,
            ImagePath = model.ImagePath,
            Tags = model.Tags,
            Muscles = model.Muscles.Select(MuscleDbEntity.FromViewModel).ToList(),
        };
    }
}
