using Mediator;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record DeleteMuscleById(Guid Id) : ICommand;
public sealed class DeleteMuscleByIdHandler : ICommandHandler<DeleteMuscleById>
{
    private readonly MusclesContainer _container;
    private readonly ILogger<DeleteMuscleByIdHandler> _logger;

    public DeleteMuscleByIdHandler(MusclesContainer container, ILogger<DeleteMuscleByIdHandler> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(DeleteMuscleById command, CancellationToken cancellationToken)
    {
        await _container.DeleteEntity<Muscle>(command.Id, _logger);
        return Unit.Value;
    }
}