using FizzWare.NBuilder;
using Mediator;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;
using WorkoutTracker.Services.Statistics;

namespace WorkoutTracker.UnitTests;

[TestFixture]
public class DataSyncServiceTests
{
    private IWorkoutDataProvider _db;
    private IWorkoutRepository _repository;
    private IMediator _mediator;
    private DataSyncService _service;
    
    [SetUp]
    public void Init()
    {        
        _db = Substitute.For<IWorkoutDataProvider>();
        _repository = Substitute.For<IWorkoutRepository>();
        var logger = Substitute.For<ILogger<DataSyncService>>();
        _mediator = Substitute.For<IMediator>();
        _service = new DataSyncService(_mediator, _db, _repository, logger);
    }

    [Test]
    public async Task ShouldGatherAllStatsAndSave()
    {
        var summary = new WorkoutsSummary(10, 1, 5);
        var byMuscleGroup = Builder<DataSeriesItem>
            .CreateListOfSize(14)
            .All()
            .WithFactory(i=> new DataSeriesItem($"M{i}", i))
            .Build();

        _mediator.Send(Arg.Any<GetWorkoutsSummary>()).Returns(summary);
        _mediator.Send(Arg.Any<GetPercentageByMuscleGroupStats>()).Returns(byMuscleGroup);

        await _service.UpdateStatistics();

        _db.Received(1).UpdateWorkoutStatistics(new WorkoutStatistics(summary, null, byMuscleGroup));
    }
}