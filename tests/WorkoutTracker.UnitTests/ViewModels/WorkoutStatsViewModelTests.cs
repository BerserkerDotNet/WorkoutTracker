using FizzWare.NBuilder;
using FluentAssertions;
using NSubstitute;
using System.Collections.ObjectModel;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.Models;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.UnitTests.ViewModels;

[TestFixture]
public class WorkoutStatsViewModelTests
{
    private WorkoutStatsViewModel _viewModel;
    private IWorkoutDataProvider _trackerDb;
    
    [SetUp]
    public void SetUp()
    {
        _trackerDb = Substitute.For<IWorkoutDataProvider>();
        _viewModel = new WorkoutStatsViewModel(_trackerDb);
    }
    
    [Test]
    public void InitShouldSetupPropertiesCorrectly()
    {
        var summary = new WorkoutsSummary(10, 8, 5);
        var timeMetrics = new WorkoutTimeMetrics(TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero, TimeSpan.Zero);
        var series = new[]
        {
            new DataSeriesItem("Foo", 12),
            new DataSeriesItem("Bla", 12),
            
        };
        var stats = new WorkoutStatistics(summary, timeMetrics, series);
        _trackerDb.GetWorkoutStatistics().Returns(stats);

        var logs =Builder<LogEntryViewModel>.CreateListOfSize(10)
            .Build();
            
        _trackerDb.GetWorkoutLogs(DateTime.Today, 7).Returns(logs);

        _viewModel.Init();

        _viewModel.WorkoutsSummary.Should().Be(stats.Summary);
        _viewModel.TimeMetrics.Should().Be(stats.TimeMetrics);
        _viewModel.MuscleGroupStats.Should().Equal(stats.PercentagePerMuscleGroup);
        _viewModel.WorkoutHistory.Should().Equal(logs);
    }
    
    [Test]
    public void LoadMoreLogsShouldAddLogsCorrectly()
    {
        var oldestDate = new DateTime(2023, 08, 31);
        var initialLogs = Builder<LogEntryViewModel>.CreateListOfSize(10)
            .TheFirst(5)
            .With(d=>d.Date = new DateTime(2023, 08, 31))
            .TheRest()
            .With(d=>d.Date = new DateTime(2023, 09, 02))
            .Build();

        _viewModel.WorkoutHistory = new ObservableCollection<LogEntryViewModel>(initialLogs);
        
        var newLogs = Builder<LogEntryViewModel>.CreateListOfSize(8)
            .All()
            .With(d=>d.Date = new DateTime(2023, 08, 25))
            .Build();
        _trackerDb.GetWorkoutLogs(oldestDate.AddDays(-1), 7).Returns(newLogs);

        _viewModel.LoadMoreLogs();

        _viewModel.WorkoutHistory.Should().Contain(newLogs);
        _viewModel.WorkoutHistory.Count.Should().Be(18);
            
        _viewModel.IsRefreshing.Should().BeFalse();
    }
    
    [Test]
    public void LoadMoreLogsShouldNotFailIfNoItemsToAdd()
    {
        var oldestDate = new DateTime(2023, 08, 31);
        var initialLogs = Builder<LogEntryViewModel>.CreateListOfSize(10)
            .TheFirst(5)
            .With(d=>d.Date = new DateTime(2023, 08, 31))
            .TheRest()
            .With(d=>d.Date = new DateTime(2023, 09, 02))
            .Build();

        _viewModel.WorkoutHistory = new ObservableCollection<LogEntryViewModel>(initialLogs);
        
        _trackerDb.GetWorkoutLogs(oldestDate.AddDays(-1), 7).Returns(Enumerable.Empty<LogEntryViewModel>());

        _viewModel.LoadMoreLogs();

        _viewModel.WorkoutHistory.Should().Contain(initialLogs);
        _viewModel.WorkoutHistory.Count.Should().Be(10);
            
        _viewModel.IsRefreshing.Should().BeFalse();
    }
}