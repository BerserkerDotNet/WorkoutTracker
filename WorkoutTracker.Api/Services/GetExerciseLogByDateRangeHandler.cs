using Mapster;
using Mediator;
using Microsoft.Azure.Cosmos.Linq;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record GetExerciseLogByDateRange(DateTime From, DateTime To) : IRequest<IEnumerable<LogEntryViewModel>>;

public sealed class GetExerciseLogByDateRangeHandler : IRequestHandler<GetExerciseLogByDateRange, IEnumerable<LogEntryViewModel>>
{
    private readonly IMediator _mediator;
    private readonly ExerciseLogsContainer _logsContainer;
    private readonly ILogger<GetExerciseLogByDateRangeHandler> _logger;

    public GetExerciseLogByDateRangeHandler(IMediator mediator, ExerciseLogsContainer logsContainer, ILogger<GetExerciseLogByDateRangeHandler> logger)
    {
        _mediator = mediator;
        _logsContainer = logsContainer;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<LogEntryViewModel>> Handle(GetExerciseLogByDateRange request, CancellationToken cancellationToken)
    {
        var logs = new List<LogEntryViewModel>();
        using var iterator = _logsContainer.GetItemLinqQueryable<ExerciseLogEntry>()
            .Where(e => e.Date >= request.From && e.Date < request.To)
            .OrderByDescending(e => e.Date)
            .ToFeedIterator();

        var exercises = await _mediator.Send(new GetAllExercises());
        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var viewModel = item.Adapt<LogEntryViewModel>();
                var exercise = exercises.Single(e => e.Id == item.ExerciseId);
                viewModel.Exercise = exercise;
                logs.Add(viewModel);
            }
        }

        return logs;
    }
}

public sealed record GetPreviousWorkoutStatsByExercise(Guid ExerciseId) : IRequest<IEnumerable<LogEntryViewModel>>;

public sealed class GetPreviousWorkoutStatsByExerciseHandler : IRequestHandler<GetPreviousWorkoutStatsByExercise, IEnumerable<LogEntryViewModel>>
{
    private readonly IMediator _mediator;
    private readonly ExerciseLogsContainer _logsContainer;
    private readonly ILogger<GetPreviousWorkoutStatsByExerciseHandler> _logger;

    public GetPreviousWorkoutStatsByExerciseHandler(IMediator mediator, ExerciseLogsContainer logsContainer, ILogger<GetPreviousWorkoutStatsByExerciseHandler> logger)
    {
        _mediator = mediator;
        _logsContainer = logsContainer;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<LogEntryViewModel>> Handle(GetPreviousWorkoutStatsByExercise request, CancellationToken cancellationToken)
    {
        var logs = new List<LogEntryViewModel>();
        var today = DateTime.Today.ToUniversalTime();
        using var iterator = _logsContainer.GetItemLinqQueryable<ExerciseLogEntry>()
            .Where(e => e.Date < today && e.ExerciseId == request.ExerciseId)
            .OrderByDescending(e => e.Date)
            .Take(1)
            .ToFeedIterator();

        var exercises = await _mediator.Send(new GetAllExercises());
        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var viewModel = item.Adapt<LogEntryViewModel>();
                var exercise = exercises.Single(e => e.Id == item.ExerciseId);
                viewModel.Exercise = exercise;
                logs.Add(viewModel);
            }
        }

        return logs;
    }
}