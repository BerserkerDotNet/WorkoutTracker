using Mediator;
using Microsoft.Azure.Cosmos.Linq;
using WorkoutTracker.Api.Data;
using WorkoutTracker.Models.Entities;

namespace WorkoutTracker.Api.Services;

public sealed record GetAllWorkoutPrograms() : IRequest<IEnumerable<WorkoutProgram>>;

public sealed class GetAllWorkoutProgramsHandler : IRequestHandler<GetAllWorkoutPrograms, IEnumerable<WorkoutProgram>>
{
    private readonly WorkoutProgramsContainer _container;

    public GetAllWorkoutProgramsHandler(WorkoutProgramsContainer container)
    {
        _container = container;
    }

    public async ValueTask<IEnumerable<WorkoutProgram>> Handle(GetAllWorkoutPrograms request, CancellationToken cancellationToken)
    {
        var allExercises = new List<WorkoutProgram>();
        using var iterator = _container.GetItemLinqQueryable<WorkoutProgram>()
            .ToFeedIterator();

        while (iterator.HasMoreResults)
        {
            foreach (var item in await iterator.ReadNextAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();
                allExercises.Add(item);
            }
        }

        return allExercises;
    }
}
