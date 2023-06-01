using FizzWare.NBuilder;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;

namespace WorkoutTracker.UnitTests;

[TestFixture]
public class DataSyncServiceTests
{
    private IWorkoutDataProvider _db;
    private IWorkoutRepository _repository;
    private TimeProvider _timeProvider;
    private DataSyncService _service;
    
    [SetUp]
    public void Init()
    {        
        _db = Substitute.For<IWorkoutDataProvider>();
        _repository = Substitute.For<IWorkoutRepository>();
        var logger = Substitute.For<ILogger<DataSyncService>>();
        _timeProvider = Substitute.For<TimeProvider>();
        _service = new DataSyncService(_db, _repository, _timeProvider, logger);
    }

    [Test]
    public void UpdateStatisticsShouldCalculateMetricsCorrectly()
    {
        var generator = new SequentialGenerator<DateTime>()
        {
            IncrementDateBy = IncrementDate.Day,
            Direction = GeneratorDirection.Ascending
        };
        generator.StartingWith(new DateTime(2023, 4, 28));
        var logs = Builder<LogEntryViewModel>
            .CreateListOfSize(18)
            .All()
                .With(x=> x.Date = generator.Generate())
            .Build();

        _db.GetWorkoutLogs().Returns(logs);
        _timeProvider.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2023, 05, 17, 23, 59, 59, TimeSpan.Zero));
        
        _service.UpdateStatistics();
        
        _db.Received(1).UpdateWorkoutStatistics(Arg.Is(new TotalWorkoutData(18, 1, 15)));
    }
    
    [Test]
    public void UpdateStatisticsShouldCountASingleWorkoutPerDay()
    {
        var day1Gen = GetByMinuteDateGenerator(new DateTime(2023, 5, 1, 08, 0, 0)); 
        var day2Gen = GetByMinuteDateGenerator(new DateTime(2023, 5, 2, 08, 0, 0)); 
        var day3Gen = GetByMinuteDateGenerator(new DateTime(2023, 5, 3, 08, 0, 0)); 
        var day4Gen = GetByMinuteDateGenerator(new DateTime(2023, 5, 4, 08, 0, 0)); 
        
        var logs = Builder<LogEntryViewModel>
            .CreateListOfSize(20)
            .TheFirst(5)
                .With(x=> x.Date = day1Gen.Generate())
            .TheNext(5)
                .With(x=> x.Date = day2Gen.Generate())
            .TheNext(5)
                .With(x=> x.Date = day3Gen.Generate())
            .TheNext(5)
                .With(x=> x.Date = day4Gen.Generate())
            .Build();

        _db.GetWorkoutLogs().Returns(logs);
        _timeProvider.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2023, 05, 6, 23, 59, 59, TimeSpan.Zero));
        
        _service.UpdateStatistics();
        
        _db.Received(1).UpdateWorkoutStatistics(Arg.Is(new TotalWorkoutData(4, 4, 4)));
    }

    [Test] public void UpdateStatisticsShouldCalculateMetricsOverMultipleMonths()
    {
        var generator = new SequentialGenerator<DateTime>()
        {
            IncrementDateBy = IncrementDate.Day,
            Direction = GeneratorDirection.Ascending
        };
        generator.StartingWith(new DateTime(2022, 02, 10));
        var logs = Builder<LogEntryViewModel>
            .CreateListOfSize(257)
            .All()
            .With(x=> x.Date = generator.Generate())
            .Build();

        _db.GetWorkoutLogs().Returns(logs);
        _timeProvider.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2023, 05, 17, 23, 59, 59, TimeSpan.Zero));
        
        _service.UpdateStatistics();
        
        _db.Received(1).UpdateWorkoutStatistics(Arg.Is(new TotalWorkoutData(257, 0, 0)));
    }

    private SequentialGenerator<DateTime> GetByMinuteDateGenerator(DateTime startTime)
    {
        var generator = new SequentialGenerator<DateTime>()
        {
            IncrementDateBy = IncrementDate.Minute,
            Direction = GeneratorDirection.Ascending,
            IncrementDateValueBy = 10
        };
        generator.StartingWith(startTime);

        return generator;
    }
}