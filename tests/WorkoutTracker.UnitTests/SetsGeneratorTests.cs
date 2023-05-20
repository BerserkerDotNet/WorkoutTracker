using FluentAssertions;
using NSubstitute;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services;
using WorkoutTracker.Services.Extensions;
using WorkoutTracker.Services.Interfaces;

namespace WorkoutTracker.UnitTests;

public class SetsGeneratorTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GenerateWarmupSetsFromHistoricalData()
    {
        const int WorkingWeight = 220;
        var expectedWorkingSet = GenerateWorkingSet(WorkingWeight, 5);

        var expectedSets = new[]
        {
            GenerateWarmupSet(45, 10),
            GenerateWarmupSet(95, 5),
            GenerateWarmupSet(145, 3),
            GenerateWarmupSet(195, 2),
            expectedWorkingSet,
            expectedWorkingSet,
            expectedWorkingSet
        };

        var fakeLog = new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            Exercise = new ExerciseViewModel(),
            Sets = new[]
            {
                new CompletedSet
                {
                    Weight = WorkingWeight,
                    Repetitions = 5,
                    CompletionTime = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    RestTime = TimeSpan.Zero
                }
            }
        };

        var provider = Substitute.For<IWorkoutDataProvider>();
        provider.GetLastEntryForExercise(fakeLog.Id).Returns(fakeLog);

        SetsGenerator generator = new SetsGenerator(provider);
        var result = generator.Generate(fakeLog.Id, new PowerLadderOverloadFactor(5, true));

        result.Should().BeEquivalentTo(expectedSets);
    }

    [Test]
    public void WorksWithLegacySets()
    {
        const int WorkingWeight = 220;
        var expectedWorkingSet = GenerateWorkingSet(WorkingWeight, 5);

        var expectedSets = new[]
        {
            GenerateWarmupSet(45, 10),
            GenerateWarmupSet(95, 5),
            GenerateWarmupSet(145, 3),
            GenerateWarmupSet(195, 2),
            expectedWorkingSet,
            expectedWorkingSet,
            expectedWorkingSet
        };

        var fakeLog = new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            Exercise = new ExerciseViewModel(),
            Sets = new[]
            {
                new LegacySet
                {
                    Weight = 100,
                    Repetitions = 5,
                    CompletionTime = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    RestTime = TimeSpan.Zero
                },
                new LegacySet
                {
                    Weight = 185,
                    Repetitions = 5,
                    CompletionTime = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    RestTime = TimeSpan.Zero
                },
                new LegacySet
                {
                    Weight = WorkingWeight,
                    Repetitions = 5,
                    CompletionTime = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    RestTime = TimeSpan.Zero
                }
            }
        };

        var provider = Substitute.For<IWorkoutDataProvider>();
        provider.GetLastEntryForExercise(fakeLog.Id).Returns(fakeLog);

        SetsGenerator generator = new SetsGenerator(provider);
        var result = generator.Generate(fakeLog.Id, new PowerLadderOverloadFactor(5, true));

        result.Should().BeEquivalentTo(expectedSets);
    }

    [Test]
    public void ShouldIncrementByStepIfRepsCountOverTarget()
    {
        const int WorkingWeight = 220;
        const int IncrementStep = 15;
        var expectedWorkingSet = GenerateWorkingSet(WorkingWeight + IncrementStep, 5);

        var expectedSets = new[]
        {
            GenerateWarmupSet(45, 10),
            GenerateWarmupSet(100, 5),
            GenerateWarmupSet(155, 3),
            GenerateWarmupSet(210, 2),
            expectedWorkingSet,
            expectedWorkingSet,
            expectedWorkingSet
        };

        var fakeLog = new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            Exercise = new ExerciseViewModel(),
            Sets = new[]
            {
                new CompletedSet
                {
                    Weight = WorkingWeight,
                    Repetitions = 6,
                    CompletionTime = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    RestTime = TimeSpan.Zero
                }
            }
        };

        var provider = Substitute.For<IWorkoutDataProvider>();
        provider.GetLastEntryForExercise(fakeLog.Id).Returns(fakeLog);

        SetsGenerator generator = new SetsGenerator(provider);
        var result = generator.Generate(fakeLog.Id, new PowerLadderOverloadFactor(IncrementStep, true));

        result.Should().BeEquivalentTo(expectedSets);
    }

    [Test]
    public void AssignsDefaultWeightIfNoPreviousData()
    {
        var expectedWorkingSet = GenerateWorkingSet(45, 5);
        var expectedSets = new[]
        {
            expectedWorkingSet,
            expectedWorkingSet,
            expectedWorkingSet
        };

        var provider = Substitute.For<IWorkoutDataProvider>();
        provider.GetLastEntryForExercise(Arg.Any<Guid>()).Returns((LogEntryViewModel)null);

        var generator = new SetsGenerator(provider);
        var result = generator.Generate(Guid.NewGuid(), new PowerLadderOverloadFactor(5, true));

        result.Should().BeEquivalentTo(expectedSets);
    }

    [TestCase(100)]
    [TestCase(150)]
    [TestCase(175)]
    [TestCase(200)]
    [TestCase(225)]
    [TestCase(300)]
    [TestCase(330)]
    [TestCase(410)]
    [TestCase(505)]
    public void MaxWarmupWeightShouldBe90PercentOfWorkingWeight(int workingWeight)
    {
        var fakeLog = new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            Exercise = new ExerciseViewModel(),
            Sets = new[]
            {
                new CompletedSet
                {
                    Weight = workingWeight,
                    Repetitions = 5,
                    CompletionTime = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    RestTime = TimeSpan.Zero
                }
            }
        };

        var provider = Substitute.For<IWorkoutDataProvider>();
        provider.GetLastEntryForExercise(fakeLog.Id).Returns(fakeLog);

        SetsGenerator generator = new SetsGenerator(provider);
        var result = generator.Generate(fakeLog.Id, new PowerLadderOverloadFactor(5, true));

        result.Where(s => s.IsWarmup).Min(s => s.Weight).Should().Be(45);
        result.Where(s => s.IsWarmup).Max(s => s.Weight).Should().BeApproximately((workingWeight * 0.9).RoundToNearestFive(), 10.0);
    }

    [TestCase(100, 4)]
    [TestCase(150, 4)]
    [TestCase(175, 4)]
    [TestCase(200, 4)]
    [TestCase(225, 4)]
    [TestCase(300, 4)]
    [TestCase(330, 4)]
    [TestCase(410, 5)]
    [TestCase(505, 6)]
    [TestCase(815, 8)]
    [TestCase(900, 9)]
    [TestCase(1010, 10)]
    public void WarmupIncrementShouldNotBeMoreThanA100(int workingWeight, int expectedWarmupSets)
    {
        var fakeLog = new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            Exercise = new ExerciseViewModel(),
            Sets = new[]
            {
                new CompletedSet
                {
                    Weight = workingWeight,
                    Repetitions = 5,
                    CompletionTime = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    RestTime = TimeSpan.Zero
                }
            }
        };

        var provider = Substitute.For<IWorkoutDataProvider>();
        provider.GetLastEntryForExercise(fakeLog.Id).Returns(fakeLog);

        SetsGenerator generator = new SetsGenerator(provider);
        var result = generator.Generate(fakeLog.Id, new PowerLadderOverloadFactor(5, true));

        var warmups = result.Where(s => s.IsWarmup).ToArray();
        warmups.Length.Should().Be(expectedWarmupSets);

        for (int i = 1; i < warmups.Length; i++)
        {
            (warmups[i].Weight - warmups[i - 1].Weight).Should().BeLessThanOrEqualTo(100);
        }
    }

    [TestCase(45, 0)]
    [TestCase(50, 0)]
    [TestCase(55, 0)]
    [TestCase(75, 0)]
    [TestCase(80, 4)]
    public void ShouldNotGenerateWarmupSetsIfWorkingWeightIsLow(int workingWeight, int expectedWarmupSets)
    {
        var fakeLog = new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            Exercise = new ExerciseViewModel(),
            Sets = new[]
            {
                new CompletedSet
                {
                    Weight = workingWeight,
                    Repetitions = 5,
                    CompletionTime = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    RestTime = TimeSpan.Zero
                }
            }
        };

        var provider = Substitute.For<IWorkoutDataProvider>();
        provider.GetLastEntryForExercise(fakeLog.Id).Returns(fakeLog);

        SetsGenerator generator = new SetsGenerator(provider);
        var result = generator.Generate(fakeLog.Id, new PowerLadderOverloadFactor(5, true));

        result.Count(s => s.IsWarmup).Should().Be(expectedWarmupSets);
    }

    [Test]
    public void ShouldNotGenerateWarmupSetsIfDisabledInDefinition()
    {
        var fakeLog = new LogEntryViewModel
        {
            Id = Guid.NewGuid(),
            Date = DateTime.Now,
            Exercise = new ExerciseViewModel(),
            Sets = new[]
            {
                new CompletedSet
                {
                    Weight = 220,
                    Repetitions = 5,
                    CompletionTime = DateTime.Now,
                    Duration = TimeSpan.Zero,
                    RestTime = TimeSpan.Zero
                }
            }
        };

        var provider = Substitute.For<IWorkoutDataProvider>();
        provider.GetLastEntryForExercise(fakeLog.Id).Returns(fakeLog);

        SetsGenerator generator = new SetsGenerator(provider);
        var result = generator.Generate(fakeLog.Id, new PowerLadderOverloadFactor(5, false));

        result.Count(s => s.IsWarmup).Should().Be(0);
    }

    private ProposedSet GenerateWarmupSet(int weight, int reps) => GenerateSet(weight, reps, true);

    private ProposedSet GenerateWorkingSet(int weight, int reps) => GenerateSet(weight, reps, false);

    private ProposedSet GenerateSet(int weight, int reps, bool isWarmup) => new ProposedSet { Repetitions = reps, Weight = weight, IsWarmup = isWarmup };
}