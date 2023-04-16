using System;
using System.Collections.Generic;
using System.Linq;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.Models.Selectors;

public class MuscleExerciseSelector : IExerciseSelector
{
    public MuscleExerciseSelector(Guid muscleId, int? targetSets = null, TimeSpan? targetRestTime = default)
    {
        this.MuscleId = muscleId;
        this.TargetSets = targetSets;
        this.TargetRestTime = targetRestTime;
    }

    public ExerciseDescriptor Select(IEnumerable<ExerciseViewModel> exercises)
    {
        var matchedExercises = exercises.Where(e =>
        {
            var muscles = e.Muscles.Select(m => m.Id).ToArray();
            return Array.IndexOf(muscles, MuscleId) == 0;
        });

        return matchedExercises.Any() ? new ExerciseDescriptor(matchedExercises, TargetSets, TargetRestTime) : null;
    }
    
    public string DisplayText => "Specific muscle";

    public Guid MuscleId { get; set; }
    public int? TargetSets { get; set; }
    public TimeSpan? TargetRestTime { get; set; }

    public void Deconstruct(out Guid muscleId, out int? targetSets, out TimeSpan? targetRestTime)
    {
        muscleId = this.MuscleId;
        targetSets = this.TargetSets;
        targetRestTime = this.TargetRestTime;
    }
}

public class SpecificExerciseSelector : IExerciseSelector
{
    public SpecificExerciseSelector(Guid exerciseId, int? targetSets = null, TimeSpan? targetRestTime = default)
    {
        this.ExerciseId = exerciseId;
        this.TargetSets = targetSets;
        this.TargetRestTime = targetRestTime;
    }

    public ExerciseDescriptor Select(IEnumerable<ExerciseViewModel> exercises)
    {
        var matchedExercises = exercises.Where(e => e.Id == ExerciseId);
        return matchedExercises.Any() ? new ExerciseDescriptor(matchedExercises, TargetSets, TargetRestTime) : null;
    }

    public string DisplayText => "Specific exercise";

    public Guid ExerciseId { get; set; }
    public int? TargetSets { get; set; }
    public TimeSpan? TargetRestTime { get; set; }

    public void Deconstruct(out Guid exerciseId, out int? targetSets, out TimeSpan? targetRestTime)
    {
        exerciseId = this.ExerciseId;
        targetSets = this.TargetSets;
        targetRestTime = this.TargetRestTime;
    }
}

public class MuscleGroupExerciseSelector : IExerciseSelector
{
    public MuscleGroupExerciseSelector(string groupName, int? targetSets = null, TimeSpan? targetRestTime = default)
    {
        this.GroupName = groupName;
        this.TargetSets = targetSets;
        this.TargetRestTime = targetRestTime;
    }
    
    public string DisplayText => "Muscle group";

    public ExerciseDescriptor Select(IEnumerable<ExerciseViewModel> exercises)
    {
        var matchedExercises = exercises.Where(e => Array.IndexOf(e.MuscleGroups, GroupName) >= 0 && Array.IndexOf(e.MuscleGroups, GroupName) < 3);
        return matchedExercises.Any() ? new ExerciseDescriptor(matchedExercises, TargetSets, TargetRestTime) : null;
    }

    public static implicit operator MuscleGroupExerciseSelector(string group) => new MuscleGroupExerciseSelector(group);
    public string GroupName { get; set; }
    public int? TargetSets { get; set; }
    public TimeSpan? TargetRestTime { get; set; }

    public void Deconstruct(out string groupName, out int? targetSets, out TimeSpan? targetRestTime)
    {
        groupName = this.GroupName;
        targetSets = this.TargetSets;
        targetRestTime = this.TargetRestTime;
    }
}