using Mediator;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record CreateOrUpdateExercise(ExerciseViewModel Model) : ICommand<ExerciseViewModel>;
public sealed class CreateOrUpdateExerciseHandler : ICommandHandler<CreateOrUpdateExercise, ExerciseViewModel>
{
    private readonly ExercisesContainer _container;
    private readonly ILogger<CreateOrUpdateExerciseHandler> _logger;

    public CreateOrUpdateExerciseHandler(ExercisesContainer container, ILogger<CreateOrUpdateExerciseHandler> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async ValueTask<ExerciseViewModel> Handle(CreateOrUpdateExercise command, CancellationToken cancellationToken)
    {
        var entity = Exercise.FromViewModel(command.Model);
        var response = await _container.UpsertEntity(entity, _logger);
        var result = response.ToViewModel(command.Model.Muscles);
        return result;
    }
}