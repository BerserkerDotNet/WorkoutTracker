using Mediator;
using WorkoutTracker.Models.Presentation;

namespace WorkoutTracker.Services.Statistics;

public record BaseStatisticsRequest<T>(IEnumerable<LogEntryViewModel> Logs): IRequest<T>;