using Mediator;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record GetExerciseById(Guid Id) : IRequest<ExerciseViewModel>;

public sealed class GetExerciseByIdHandler : IRequestHandler<GetExerciseById, ExerciseViewModel>
{
    private readonly IMediator _mediator;
    private readonly ExercisesContainer _container;
    private readonly ILogger<GetExerciseByIdHandler> _logger;

    public GetExerciseByIdHandler(IMediator mediator, ExercisesContainer container, ILogger<GetExerciseByIdHandler> logger)
    {
        _mediator = mediator;
        _container = container;
        _logger = logger;
    }

    public async ValueTask<ExerciseViewModel> Handle(GetExerciseById request, CancellationToken cancellationToken)
    {
        var allMuscles = await _mediator.Send(new GetAllMuscles());

        var item = await _container.ReadItemAsync<Exercise>(request.Id.ToString(), new PartitionKey(request.Id.ToString()));
        var viewModel = item.Resource.ToViewModel(allMuscles);

        return viewModel;
    }
}