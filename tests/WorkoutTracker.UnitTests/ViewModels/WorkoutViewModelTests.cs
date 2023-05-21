using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Collections.ObjectModel;
using UnitsNet;
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
    private ISetsGenerator _setsGenerator;
    
    [SetUp]
    public void Init()
    {
        _dataProvider = Substitute.For<IWorkoutDataProvider>();
        _setsGenerator = Substitute.For<ISetsGenerator>();
        var timer = Substitute.For<IExerciseTimerService>();
        var context = new ApplicationContext<WorkoutViewModel>(Substitute.For<INotificationService>(),
            Substitute.For<ILogger<WorkoutViewModel>>());
        _vm = new WorkoutViewModel(_dataProvider, _setsGenerator, timer, context);
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

    [TestCase(true)]
    [TestCase(false)]
    public void ReplaceExerciseAddsIfSelectedModelIsNull(bool includeWarmup)
    {
        _setsGenerator.Generate(Arg.Any<Guid>(), Arg.Any<IProgressiveOverloadFactor>())
            .Returns(new[]
            {
                new ProposedSet() { Weight = 10, Repetitions = 10 },
                new ProposedSet() { Weight = 10, Repetitions = 10 },
            });
        var exercise = new ExerciseViewModel()
        {
            Id = Guid.NewGuid()
        };
        
        var options = new AddReplaceExerciseOptions(exercise, includeWarmup);
        _vm.SelectedModel = null;
        
        _vm.ReplaceExercise(options);

        _vm.TodaySets.Should().HaveCount(1);
        var workout = _vm.TodaySets.First();
        workout.Exercise.Should().Be(exercise);
        workout.Sets.Should().HaveCount(2);
        
        _dataProvider.Received(1).UpdateViewModel(workout);
        _setsGenerator.Received(1).Generate(exercise.Id,
            Arg.Is<PowerLadderOverloadFactor>(o => o.IncludeWarmup == includeWarmup && o.StepIncrement == 5));
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void ReplaceExerciseIfSelectedModelIsNotNull(bool includeWarmup)
    {
        _setsGenerator.Generate(Arg.Any<Guid>(), Arg.Any<IProgressiveOverloadFactor>())
            .Returns(new[]
            {
                new ProposedSet() { Weight = 10, Repetitions = 10 },
            });
        var newExercise = new ExerciseViewModel()
        {
            Id = Guid.NewGuid()
        };
        var options = new AddReplaceExerciseOptions(newExercise, includeWarmup);
        _vm.TodaySets.Add(new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Exercise = new ExerciseViewModel(),
            Sets = Enumerable.Empty<IExerciseSet>(),
            Date = DateTime.UtcNow,
            Order = 0
        });
        _vm.TodaySets.Add(new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Exercise = new ExerciseViewModel(),
            Sets = Enumerable.Empty<IExerciseSet>(),
            Date = DateTime.UtcNow,
            Order = 1
        });
        _vm.TodaySets.Add(new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Exercise = new ExerciseViewModel(),
            Sets = Enumerable.Empty<IExerciseSet>(),
            Date = DateTime.UtcNow,
            Order = 2
        });

        _vm.SelectedModel = _vm.TodaySets.ElementAt(1);
        
        _vm.ReplaceExercise(options);

        _vm.TodaySets.Should().HaveCount(3);
        _vm.SelectedModel.Should().BeNull();
        var workout = _vm.TodaySets.ElementAt(1);
        workout.Exercise.Should().Be(newExercise);
        workout.Sets.Should().HaveCount(1);
        workout.Order.Should().Be(1);
        
        _dataProvider.Received(1).UpdateViewModel(workout);
        _setsGenerator.Received(1).Generate(newExercise.Id,
            Arg.Is<PowerLadderOverloadFactor>(o => o.IncludeWarmup == includeWarmup && o.StepIncrement == 5));
    }
}