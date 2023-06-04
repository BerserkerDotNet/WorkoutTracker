using FizzWare.NBuilder;
using FluentAssertions;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Statistics;

namespace WorkoutTracker.UnitTests.Statistics;

[TestFixture]
public class GetPercentageByMuscleGroupStatsHandlerTests
{
    private GetPercentageByMuscleGroupStatsHandler _handler;
    
    [SetUp]
    public void Init()
    {        
        _handler = new GetPercentageByMuscleGroupStatsHandler();
    }

    [Test]
    public async Task ShouldGenerateCorrectPercentagesBasedOnMuscleGroups()
    {
        const string chestMuscleGroup = "Chest";
        var chestExercise = new ExerciseViewModel()
        {
            Muscles = new MuscleViewModel[] { new() { MuscleGroup = chestMuscleGroup } }
        };

        const string legsMuscleGroup = "Legs";
        var legExercise = new ExerciseViewModel()
        {
            Muscles = new MuscleViewModel[] { new() { MuscleGroup = legsMuscleGroup } }
        };

        const string backMuscleGroup = "Back";
        var backExercise = new ExerciseViewModel()
        {
            Muscles = new MuscleViewModel[] { new() { MuscleGroup = backMuscleGroup } }
        };
        
        var sets = Builder<CompletedSet>.CreateListOfSize(4).Build();

        var logs = Builder<LogEntryViewModel>
            .CreateListOfSize(20)
            .All()
            .With(l => l.Sets = sets)
            .Random(10)
            .With(l => l.Exercise = chestExercise)
            .Random(7)
            .With(l => l.Exercise = legExercise)
            .Random(3)
            .With(l => l.Exercise = backExercise)
            .Build();
        
        var result = await _handler.Handle(new GetPercentageByMuscleGroupStats(logs), CancellationToken.None);

        result.Should().HaveCount(3);

        result.First(d => d.Name == chestMuscleGroup).Value.Should().Be(50);
        result.First(d => d.Name == legsMuscleGroup).Value.Should().Be(35);
        result.First(d => d.Name == backMuscleGroup).Value.Should().Be(15);

    }
    
    [Test]
    public async Task ShouldCountAllMusclesInTheExercise()
    {
        const string chestMuscleGroup = "Chest";
        const string legsMuscleGroup = "Legs";
        const string backMuscleGroup = "Back";
        var compoundExercise = new ExerciseViewModel()
        {
            Muscles = new MuscleViewModel[] { new() { MuscleGroup = chestMuscleGroup }, new() { MuscleGroup = legsMuscleGroup } , new() { MuscleGroup = backMuscleGroup }  }
        };

        var sets = Builder<CompletedSet>.CreateListOfSize(4).Build();

        var logs = Builder<LogEntryViewModel>
            .CreateListOfSize(20)
            .All()
            .With(l => l.Sets = sets)
            .With(l => l.Exercise = compoundExercise)
            .Build();
        
        var result = await _handler.Handle(new GetPercentageByMuscleGroupStats(logs), CancellationToken.None);

        result.Should().HaveCount(3);

        result.First(d => d.Name == chestMuscleGroup).Value.Should().Be(33);
        result.First(d => d.Name == legsMuscleGroup).Value.Should().Be(33);
        result.First(d => d.Name == backMuscleGroup).Value.Should().Be(33);
    }
    
    [Test]
    public async Task ShouldReturnEmptyIfNoLogs()
    {
        var result =await _handler.Handle(new GetPercentageByMuscleGroupStats(Enumerable.Empty<LogEntryViewModel>()), CancellationToken.None);
        result.Should().BeEmpty();
    }

    [Test] 
    public async Task ShouldNotThrowIfLogsAreNull()
    {
        var result = await _handler.Handle(new GetPercentageByMuscleGroupStats(null), CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Test]
    public async Task ShouldOnlyIncludeCompletedWorkouts()
    {
        const string chestMuscleGroup = "Chest";
        var chestExercise = new ExerciseViewModel()
        {
            Muscles = new MuscleViewModel[] { new() { MuscleGroup = chestMuscleGroup } }
        };

        const string legsMuscleGroup = "Legs";
        var legExercise = new ExerciseViewModel()
        {
            Muscles = new MuscleViewModel[] { new() { MuscleGroup = legsMuscleGroup } }
        };

        const string backMuscleGroup = "Back";
        var backExercise = new ExerciseViewModel()
        {
            Muscles = new MuscleViewModel[] { new() { MuscleGroup = backMuscleGroup } }
        };
        
        var fullyCompletedWorkout = Builder<CompletedSet>
            .CreateListOfSize(7)
            .TheFirst(3)
            .With(s=> s.IsWarmup = true)
            .TheRest()
            .With(s=> s.IsWarmup = false)
            .Build();

        var missedWarmupWorkout = new IExerciseSet[]
        {
            Builder<ProposedSet>.CreateNew().Build(),
            Builder<ProposedSet>.CreateNew().Build(),
            Builder<ProposedSet>.CreateNew().Build(),
            Builder<ProposedSet>.CreateNew().Build(),
            Builder<CompletedSet>.CreateNew().Build(),
            Builder<CompletedSet>.CreateNew().Build(),
            Builder<CompletedSet>.CreateNew().Build(),
        };
        var missedFewSetsWorkout = new IExerciseSet[]
        {
            Builder<CompletedSet>.CreateNew().Build(),
            Builder<ProposedSet>.CreateNew().Build(),
            Builder<CompletedSet>.CreateNew().Build(),
            Builder<ProposedSet>.CreateNew().Build(),
        };
        
        var notStartedWorkout = Builder<ProposedSet>
            .CreateListOfSize(7)
            .TheFirst(3)
            .With(s=> s.IsWarmup = true)
            .TheRest()
            .With(s=> s.IsWarmup = false)
            .Build();

        var logs = Builder<LogEntryViewModel>
            .CreateListOfSize(20)
            .Random(5)
            .With(l => l.Sets = missedWarmupWorkout)
            .With(l => l.Exercise = chestExercise)
            .Random(7)
            .With(l => l.Sets = notStartedWorkout)
            .With(l => l.Exercise = legExercise)
            .Random(3)
            .With(l => l.Sets = fullyCompletedWorkout)
            .With(l => l.Exercise = backExercise)
            .Random(5)
            .With(l => l.Sets = missedFewSetsWorkout)
            .With(l => l.Exercise = legExercise)
            .Build();
        
        var result = await _handler.Handle(new GetPercentageByMuscleGroupStats(logs), CancellationToken.None);

        result.Should().HaveCount(3);

        result.First(d => d.Name == chestMuscleGroup).Value.Should().Be(38);
        result.First(d => d.Name == legsMuscleGroup).Value.Should().Be(38);
        result.First(d => d.Name == backMuscleGroup).Value.Should().Be(23);
    }
}