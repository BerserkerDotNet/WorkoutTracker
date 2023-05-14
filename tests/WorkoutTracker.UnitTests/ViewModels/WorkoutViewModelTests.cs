using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.ObjectModel;
using WorkoutTracker.MAUI.Interfaces;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Interfaces;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.UnitTests.ViewModels;

[TestFixture]
public class WorkoutViewModelTests
{
    private WorkoutViewModel _vm;
    private IWorkoutDataProvider _dataProvider;
    
    [SetUp]
    public void Init()
    {
        _dataProvider = Substitute.For<IWorkoutDataProvider>();
        var timer = Substitute.For<IExerciseTimerService>();
        var context = new ApplicationContext<WorkoutViewModel>(Substitute.For<INotificationService>(),
            Substitute.For<ILogger<WorkoutViewModel>>());
        _vm = new WorkoutViewModel(_dataProvider, new SetsGenerator(_dataProvider), timer, context);
    }

    [Test]
    public void AddingSetsShouldPersistTheRecord()
    {
        var model = new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Sets = new ObservableCollection<IExerciseSet>(),
            Order = 0,
            Exercise = new ExerciseViewModel()
        };
        
        _vm.AddSet(model);

        model.Sets.Should().HaveCount(1);
        model.Sets.Should().AllBeOfType<ProposedSet>();

        _dataProvider.Received(1).UpdateViewModel(model);
    }

    [Test]
    public void RemovingSetsShouldPersistTheRecord()
    {
        var initialSets = new ObservableCollection<IExerciseSet>();
        initialSets.Add(new ProposedSet{Weight = 100, Repetitions = 10});
        initialSets.Add(new ProposedSet{Weight = 150, Repetitions = 5});
        initialSets.Add(new ProposedSet{Weight = 200, Repetitions = 2});
        var model = new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Sets = initialSets,
            Order = 0,
            Exercise = new ExerciseViewModel()
        };
        
        _vm.ReduceSets(model);

        model.Sets.Should().HaveCount(2);
        model.Sets.Should().AllBeOfType<ProposedSet>();

        _dataProvider.Received(1).UpdateViewModel(model);
    }
}