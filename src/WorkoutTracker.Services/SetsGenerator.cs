using WorkoutTracker.MAUI.Interfaces;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Services.Extensions;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.Services;

public class SetsGenerator
{
    private readonly IWorkoutDataProvider _dataProvider;

    public SetsGenerator(IWorkoutDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public IEnumerable<IExerciseSet> Generate(Guid exerciseId, IProgressiveOverloadFactor overloadFactor)
    {
        return overloadFactor switch
        {
            OneRepMaxProgressiveOverloadFactor oneRepMax => GenerateWith1RMPercentage(exerciseId, oneRepMax),
            SteadyStateProgressiveOverloadFactor steady => GenerateSets(steady.NumberOfSets, steady.Weight, steady.NumberOfReps),
            PowerLadderOverloadFactor power => GeneratePowerLadder(exerciseId, power),
            RepetitionsLadderOverloadFactor reps => GenerateRepsLadder(exerciseId, reps),
            _ => Enumerable.Empty<IExerciseSet>()
        };
    }

    private IEnumerable<ProposedSet> GenerateSets(int count, double weight, int reps) => Enumerable.Range(0, count)
            .Select(_ => new ProposedSet { Repetitions = reps, Weight = weight })
            .ToArray();

    private IEnumerable<IExerciseSet> GenerateRepsLadder(Guid exerciseId, RepetitionsLadderOverloadFactor power)
    {
        var result = new List<ProposedSet>(10);
        var workingReps = power.StartingReps;
        var lastWorkingWeight = 0.0;
        var lastEntry = _dataProvider.GetLastEntryForExercise(exerciseId);
        if (lastEntry is not null)
        {
            var bestWorkingSet = lastEntry.Sets
                .Where(s => s is LegacySet || s is CompletedSet)
                .Where(s => !s.IsWarmup)
                .MaxBy(s => s.Repetitions);
            workingReps = bestWorkingSet.Repetitions + 1;
            lastWorkingWeight = bestWorkingSet.Weight;
        }

        if (power.IncludeWarmup)
        {
            result.AddRange(GenerateWarmupSets(lastWorkingWeight));
        }
        
        result.AddRange(GenerateSets(power.WorkingSets, lastWorkingWeight, workingReps));
        return result;
    }

    private IEnumerable<IExerciseSet> GeneratePowerLadder(Guid exerciseId, PowerLadderOverloadFactor power)
    {
        var result = new List<ProposedSet>(10);
        var workingWeight = 45.0;
        var lastEntry = _dataProvider.GetLastEntryForExercise(exerciseId);
        if (lastEntry is not null)
        {
            var bestWorkingSet = lastEntry.Sets
                .Where(s => s is LegacySet || s is CompletedSet)
                .Where(s => !s.IsWarmup)
                .MaxBy(s => s.Weight + s.Repetitions);
            workingWeight = bestWorkingSet.Repetitions > power.WorkingReps ? bestWorkingSet.Weight + power.StepIncrement : bestWorkingSet.Weight;
        }

        if (power.IncludeWarmup)
        {
            result.AddRange(GenerateWarmupSets(workingWeight));
        }

        for (var i = 0; i < power.WorkingSets; i++)
        {
            result.Add(new ProposedSet
            {
                Repetitions = power.WorkingReps,
                Weight = workingWeight
            });
        }

        return result;
    }

    private IEnumerable<ProposedSet> GenerateWarmupSets(double workingWeight)
    {
        if (workingWeight >= 80)
        {
            var warmupWeightMax = ((int)(workingWeight * 0.9)).RoundToNearestFive(); // 90% of working weight
            var warmupSetsCount = Math.Max(3, (int)Math.Ceiling((warmupWeightMax - 45.0) / 100));
            var warmupStepIncrement = ((int)Math.Ceiling((warmupWeightMax - 45.0) / warmupSetsCount)).RoundToNearestFive();
            var warmUpWeight = 45;
            yield return new ProposedSet
            {
                IsWarmup = true,
                Repetitions = 10,
                Weight = warmUpWeight
            };

            for (var i = 0; i < warmupSetsCount; i++)
            {
                warmUpWeight = Math.Min(warmUpWeight + warmupStepIncrement, warmupWeightMax);
                yield return new ProposedSet
                {
                    IsWarmup = true,
                    Repetitions = i == 0 ? 5 : (i == 1 ? 3 : 2),
                    Weight = warmUpWeight
                };
            }
        }
    }

    private IEnumerable<IExerciseSet> GenerateWith1RMPercentage(Guid exerciseId, OneRepMaxProgressiveOverloadFactor oneRepMaxProgressiveOverload)
    {
        var repsCount = (int)Math.Floor(37.0 - ((oneRepMaxProgressiveOverload.Percentage / 100.0) * 36.0));
        var maxSet = _dataProvider.GetMaxWeightLiftedOnExercise(exerciseId);

        if (maxSet is null)
        {
            // need defaults for types of exercise
            maxSet = new ProposedSet
            {
                Weight = 20,
                Repetitions = 10
            };
        }

        var oneRM = Math.Floor(maxSet.Weight * (36.0 / (37.0 - maxSet.Repetitions)));
        var weightToSet = oneRM * (oneRepMaxProgressiveOverload.Percentage / 100.0);

        // TODO: add warm-up sets

        return Enumerable.Range(0, oneRepMaxProgressiveOverload.NumberOfSets)
            .Select(_ => new ProposedSet { Repetitions = repsCount, Weight = ((int)weightToSet).RoundToNearestFive() })
            .ToArray();
    }
}
