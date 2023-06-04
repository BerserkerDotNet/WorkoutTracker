using Mediator;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.Services.Statistics;

public record GetPercentageByMuscleGroupStats(IEnumerable<LogEntryViewModel> Logs) : BaseStatisticsRequest<IEnumerable<DataSeriesItem>>(Logs);
    
public sealed class GetPercentageByMuscleGroupStatsHandler : IRequestHandler<GetPercentageByMuscleGroupStats, IEnumerable<DataSeriesItem>>
{
    public ValueTask<IEnumerable<DataSeriesItem>> Handle(GetPercentageByMuscleGroupStats request, CancellationToken cancellationToken)
    {
        if (request.Logs is null)
        {
            return ValueTask.FromResult(Enumerable.Empty<DataSeriesItem>());
        }

        var completedWorkouts = request.Logs
            .Where(l => l.Sets.OfType<CompletedSet>().Any())
            .ToArray();

        var stats = new Dictionary<string, float>();
        foreach (var log in completedWorkouts)
        {
            foreach (string muscleGroup in log.Exercise.MuscleGroups)
            {
                stats.TryAdd(muscleGroup, 0);
                stats[muscleGroup]++;
            }
        }

        var totalSum = stats.Values.Sum();
        var byMuscleGroup = stats.Select(s => new DataSeriesItem(s.Key, (int)Math.Floor((s.Value / totalSum) * 100)));

        return ValueTask.FromResult(byMuscleGroup);
    }
}