using Mapster;
using Mediator;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record CreateOrUpdateLog(LogEntryViewModel Model) : ICommand<LogEntryViewModel>;

public sealed class CreateOrUpdateLogHandler : ICommandHandler<CreateOrUpdateLog, LogEntryViewModel>
{
    private readonly ExerciseLogsContainer _container;
    private readonly ILogger<CreateOrUpdateLogHandler> _logger;

    public CreateOrUpdateLogHandler(ExerciseLogsContainer container, ILogger<CreateOrUpdateLogHandler> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async ValueTask<LogEntryViewModel> Handle(CreateOrUpdateLog command, CancellationToken cancellationToken)
    {
        var entity = command.Model.Adapt<ExerciseLogEntry>();
        var response = await _container.UpsertEntity(entity, _logger);
        var result = response.Adapt<LogEntryViewModel>();
        result.Exercise = command.Model.Exercise;

        return result;
    }
}