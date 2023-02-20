using Mediator;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record CreateOrUpdateWorkoutProgram(WorkoutProgram Program) : ICommand<WorkoutProgram>;

public sealed class CreateOrUpdateWorkoutProgramHandler : ICommandHandler<CreateOrUpdateWorkoutProgram, WorkoutProgram>
{
    private readonly WorkoutProgramsContainer _container;
    private readonly ILogger<CreateOrUpdateWorkoutProgramHandler> _logger;

    public CreateOrUpdateWorkoutProgramHandler(WorkoutProgramsContainer container, ILogger<CreateOrUpdateWorkoutProgramHandler> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async ValueTask<WorkoutProgram> Handle(CreateOrUpdateWorkoutProgram command, CancellationToken cancellationToken)
    {
        var response = await _container.UpsertEntity(command.Program, _logger);
        return response ?? null;
    }
}