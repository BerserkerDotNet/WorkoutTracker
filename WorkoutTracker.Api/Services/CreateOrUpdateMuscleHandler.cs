using Mapster;
using Mediator;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Api.Extensions;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record CreateOrUpdateMuscle(MuscleViewModel Model) : ICommand<MuscleViewModel>;
public sealed class CreateOrUpdateMuscleHandler : ICommandHandler<CreateOrUpdateMuscle, MuscleViewModel>
{
    private readonly MusclesContainer _container;
    private readonly ILogger<CreateOrUpdateMuscleHandler> _logger;

    public CreateOrUpdateMuscleHandler(MusclesContainer container, ILogger<CreateOrUpdateMuscleHandler> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async ValueTask<MuscleViewModel> Handle(CreateOrUpdateMuscle command, CancellationToken cancellationToken)
    {
        var entity = command.Model.Adapt<Muscle>();
        var response = await _container.UpsertEntity(entity, _logger);
        var result = response.Adapt<MuscleViewModel>();
        return result;
    }
}
