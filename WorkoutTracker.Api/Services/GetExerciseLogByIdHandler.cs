using Mapster;
using Mediator;
using Microsoft.Azure.Cosmos;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record GetExerciseLogById(Guid Id) : IRequest<LogEntryViewModel>;

public sealed class GetExerciseLogByIdHandler : IRequestHandler<GetExerciseLogById, LogEntryViewModel>
{
    private readonly IMediator _mediator;
    private readonly ExerciseLogsContainer _logsContainer;
    private readonly ILogger<GetExerciseLogByIdHandler> _logger;

    public GetExerciseLogByIdHandler(IMediator mediator, ExerciseLogsContainer logsContainer, ILogger<GetExerciseLogByIdHandler> logger)
    {
        _mediator = mediator;
        _logsContainer = logsContainer;
        _logger = logger;
    }

    public async ValueTask<LogEntryViewModel> Handle(GetExerciseLogById request, CancellationToken cancellationToken)
    {
        var item = await _logsContainer.ReadItemAsync<ExerciseLogEntry>(request.Id.ToString(), new PartitionKey(request.Id.ToString()));
        var exercise = await _mediator.Send(new GetExerciseById(item.Resource.ExerciseId));
        var result = item.Resource.Adapt<LogEntryViewModel>();
        result.Exercise = exercise;
        return result;
    }
}
