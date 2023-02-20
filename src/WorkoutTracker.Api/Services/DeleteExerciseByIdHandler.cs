using Mediator;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record DeleteExerciseById(Guid Id) : ICommand;
public sealed class DeleteExerciseByIdHandler : ICommandHandler<DeleteExerciseById>
{
    private readonly ExercisesContainer _container;
    private readonly ILogger<DeleteExerciseByIdHandler> _logger;

    public DeleteExerciseByIdHandler(ExercisesContainer container, ILogger<DeleteExerciseByIdHandler> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async ValueTask<Unit> Handle(DeleteExerciseById command, CancellationToken cancellationToken)
    {
        await _container.DeleteEntity<Exercise>(command.Id, _logger);
        return Unit.Value;
    }
}