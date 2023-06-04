using Mediator;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.Services.Statistics;

public record GetWorkoutsSummary(IEnumerable<LogEntryViewModel> Logs) : BaseStatisticsRequest<WorkoutsSummary>(Logs);
    
public sealed class GetWorkoutsSummaryHandler : IRequestHandler<GetWorkoutsSummary, WorkoutsSummary>
{
    private readonly TimeProvider _timeProvider;

    public GetWorkoutsSummaryHandler(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    public ValueTask<WorkoutsSummary> Handle(GetWorkoutsSummary request, CancellationToken cancellationToken)
    {
        var today = _timeProvider.GetLocalNow().Date;
        
        int daysSinceMonday = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var beginningOfThisMonth = new DateTime(today.Year, today.Month, 1);
        var beginningOfThisWeek = today.AddDays(-daysSinceMonday);
        var logs = request.Logs.ToArray(); 
        var totalCount = logs
            .GroupBy(e => new DateOnly(e.Date.Year, e.Date.Month, e.Date.Day))
            .Count();
        var thisMonth = logs
            .Where(e => e.Date >= beginningOfThisMonth)
            .GroupBy(e => new DateOnly(e.Date.Year, e.Date.Month, e.Date.Day))
            .Count();
        
        var thisWeek = logs
            .Where(e => e.Date >= beginningOfThisWeek)
            .GroupBy(e => new DateOnly(e.Date.Year, e.Date.Month, e.Date.Day))
            .Count();

        return new ValueTask<WorkoutsSummary>(Task.FromResult(new WorkoutsSummary(totalCount, thisWeek, thisMonth)));
    }
}