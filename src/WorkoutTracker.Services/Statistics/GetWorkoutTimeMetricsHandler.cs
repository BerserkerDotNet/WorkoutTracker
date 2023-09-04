using Mediator;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.Services.Statistics;

public record GetWorkoutTimeMetrics(IEnumerable<LogEntryViewModel> Logs): BaseStatisticsRequest<WorkoutTimeMetrics>(Logs);
    
public sealed class GetWorkoutTimeMetricsHandler : IRequestHandler<GetWorkoutTimeMetrics, WorkoutTimeMetrics>
{
    public ValueTask<WorkoutTimeMetrics> Handle(GetWorkoutTimeMetrics request, CancellationToken cancellationToken)
    {
        var completedWorkouts = request.Logs
            .Where(l => l.Sets.OfType<CompletedSet>().Any() || l.Sets.OfType<LegacySet>().Any())
            .ToArray();

        var totalTime = TimeSpan.Zero; 
        var totalRest = TimeSpan.Zero;
        var avgRestPerSet = TimeSpan.Zero;
        int actualWorkoutsCount = 0;
        int actualSetsCount = 0;
        foreach (var workout in completedWorkouts)
        {
            var completedSets = workout.Sets
                .Select(s =>
                {
                    return s switch
                    {
                        LegacySet ls => new { ls.CompletionTime, ls.RestTime },
                        CompletedSet cs => new { cs.CompletionTime, cs.RestTime },
                        _ => null
                    };
                })
                .Where(s => s != null)
                .OrderBy(s => s.CompletionTime)
                .ToArray();

            if (completedSets.Count() < 2)
            {
                continue;
            }
            
            var workoutDuration = completedSets.Last().CompletionTime - workout.Date;
            var totalRestTime = completedSets.Sum(s => s.RestTime.TotalSeconds);
            totalTime += workoutDuration;
            totalRest += TimeSpan.FromSeconds(totalRestTime);
            avgRestPerSet += TimeSpan.FromSeconds(completedSets.Average(s => s.RestTime.TotalSeconds));
            actualWorkoutsCount++;
        }

        var avgDuration =  TimeSpan.Zero;
        var avgRest = TimeSpan.Zero;
        if (actualWorkoutsCount > 0)
        {
            avgDuration = totalTime / actualWorkoutsCount;
            avgRest = avgRestPerSet / actualWorkoutsCount;
        }

        return ValueTask.FromResult(new WorkoutTimeMetrics(totalTime, totalRest, avgDuration, avgRest));
    }
}