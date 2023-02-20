using Mediator;
using Microsoft.Azure.Cosmos.Linq;
using WorkoutTracker.Api.Data;

namespace WorkoutTracker.Api.Services;

public sealed record GetAllMuscles : IRequest<IEnumerable<MuscleViewModel>>;

public sealed class GetAllMusclesHandler : IRequestHandler<GetAllMuscles, IEnumerable<MuscleViewModel>>
{
    private readonly MusclesContainer _container;
    private readonly ILogger<GetAllMusclesHandler> _logger;

    public GetAllMusclesHandler(MusclesContainer container, ILogger<GetAllMusclesHandler> logger)
    {
        _container = container;
        _logger = logger;
    }

    public async ValueTask<IEnumerable<MuscleViewModel>> Handle(GetAllMuscles request, CancellationToken cancellationToken)
    {
        var result = new List<MuscleViewModel>();
        using var iterator = _container.GetItemLinqQueryable<MuscleViewModel>().ToFeedIterator();
        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();
                result.Add(item);
            }
        }

        return result;
    }
}
