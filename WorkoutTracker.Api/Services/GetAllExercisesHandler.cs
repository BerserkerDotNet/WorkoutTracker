using Mediator;
using Microsoft.Azure.Cosmos.Linq;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record GetAllExercises() : IRequest<IEnumerable<ExerciseViewModel>>;

public sealed class GetAllExercisesHandler : IRequestHandler<GetAllExercises, IEnumerable<ExerciseViewModel>>
{
    private readonly IMediator _mediator;
    private readonly ExercisesContainer _container;
    private readonly ILogger<GetAllExercisesHandler> _logger;

    public GetAllExercisesHandler(IMediator mediator, ExercisesContainer container, ILogger<GetAllExercisesHandler> logger)
    {
        _mediator = mediator;
        _container = container;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<ExerciseViewModel>> Handle(GetAllExercises request, CancellationToken cancellationToken)
    {
        var allMuscles = await _mediator.Send(new GetAllMuscles());

        var allExercises = new List<ExerciseViewModel>();
        using var iterator = _container.GetItemLinqQueryable<Exercise>()
            .ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();

                var viewModel = item.ToViewModel(allMuscles);
                allExercises.Add(viewModel);
            }
        }

        return allExercises;
    }
}