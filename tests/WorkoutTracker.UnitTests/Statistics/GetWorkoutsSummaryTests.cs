using FizzWare.NBuilder;
using FluentAssertions;
using NSubstitute;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Models;
using WorkoutTracker.Services.Statistics;

namespace WorkoutTracker.UnitTests.Statistics;

[TestFixture]
public class GetWorkoutsSummaryTests
{
    private TimeProvider _timeProvider;
    private GetWorkoutsSummaryHandler _handler;
    
    [SetUp]
    public void Init()
    {        
        _timeProvider = Substitute.For<TimeProvider>();
        _timeProvider.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        _handler = new GetWorkoutsSummaryHandler(_timeProvider);
    }

    [Test]
    public async Task ShouldGenerateEmptyStatsIfNoLogs()
    {
        var summary = await _handler.Handle(new GetWorkoutsSummary(Enumerable.Empty<LogEntryViewModel>()), CancellationToken.None);
        
        summary.Should().Be(new WorkoutsSummary(0, 0, 0));
    }

    [Test]
    public async Task ShouldCalculateDataPerMonth()
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

        _timeProvider.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2023, 05, 17, 23, 59, 59, TimeSpan.Zero));
        
        var summary = await _handler.Handle(new GetWorkoutsSummary(logs), CancellationToken.None);
        
        summary.Should().Be(new WorkoutsSummary(18, 1, 15));
    }
    
    [Test]
    public async Task ShouldCountASingleWorkoutPerDay()
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

        _timeProvider.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2023, 05, 6, 23, 59, 59, TimeSpan.Zero));
        
        var summary = await _handler.Handle(new GetWorkoutsSummary(logs), CancellationToken.None);
        
        summary.Should().Be(new WorkoutsSummary(4, 4, 4));
    }

    [Test]
    public async Task ShouldCalculateMetricsOverMultipleMonths()
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

        _timeProvider.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(2023, 05, 17, 23, 59, 59, TimeSpan.Zero));
        
        var summary = await _handler.Handle(new GetWorkoutsSummary(logs), CancellationToken.None);
        
        summary.Should().Be(new WorkoutsSummary(257, 0, 0));
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