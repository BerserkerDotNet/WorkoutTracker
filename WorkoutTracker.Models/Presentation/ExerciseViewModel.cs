using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace WorkoutTracker.Models.Presentation;

public class ExerciseViewModel : IComparable<ExerciseViewModel>
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string Steps { get; set; }

    public string TutorialUrl { get; set; }

    public string ImagePath { get; set; }

    public IEnumerable<MuscleViewModel> Muscles { get; set; }

    public IList<string> Tags { get; set; }

    [JsonIgnore]
    public string[] MuscleGroups => Muscles.Select(m => m.MuscleGroup).Distinct().ToArray();

    public int CompareTo(ExerciseViewModel other)
    {
        if (Id == other.Id)
        {
            return 0;
        }

        return 1;
    }
}
