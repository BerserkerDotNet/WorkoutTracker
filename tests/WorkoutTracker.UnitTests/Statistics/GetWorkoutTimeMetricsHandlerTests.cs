using FluentAssertions;
using WorkoutTracker.Models.Contracts;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Statistics;

namespace WorkoutTracker.UnitTests.Statistics
{
    [TestFixture]
    public class GetWorkoutTimeMetricsHandlerTests
    {
        [Test]
        public async Task ShouldReturnCorrectData()
        {
            // Arrange
            var fakeLogs = new[] {
                new LogEntryViewModel
                {
                    Id = Guid.NewGuid(),
                    Exercise = new ExerciseViewModel(),
                    Date = new DateTime(2022, 1, 1),
                    Sets = new IExerciseSet[]
                    {
                        new LegacySet { CompletionTime = new DateTime(2022, 1, 1, 1, 0, 0), RestTime = TimeSpan.FromMinutes(1) },
                        new CompletedSet { CompletionTime = new DateTime(2022, 1, 1, 1, 30, 0), RestTime = TimeSpan.FromMinutes(2), Weight = 10, Repetitions = 10, Duration = TimeSpan.FromSeconds(10)},
                    }
                },
                new LogEntryViewModel
                {                
                    Id = Guid.NewGuid(),
                    Exercise = new ExerciseViewModel(),
                    Date = new DateTime(2022, 1, 2),
                    Sets = new IExerciseSet[]
                    {
                        new CompletedSet { CompletionTime = new DateTime(2022, 1, 2, 1, 0, 0), RestTime = TimeSpan.FromMinutes(1), Weight = 10, Repetitions = 10, Duration = TimeSpan.FromSeconds(10) },
                        new LegacySet { CompletionTime = new DateTime(2022, 1, 2, 1, 30, 0), RestTime = TimeSpan.FromMinutes(1) },
                    }
                },
            };

            var getWorkoutTimeMetricsRequest = new GetWorkoutTimeMetrics(fakeLogs);

            var handler = new GetWorkoutTimeMetricsHandler();

            // Act
            var result = await handler.Handle(getWorkoutTimeMetricsRequest, CancellationToken.None);

            // Assert
            result.TotalWorkoutTime.Should().Be(TimeSpan.FromHours(3));
            result.TotalRestTime.Should().Be(TimeSpan.FromMinutes(5));
            result.AvgWorkoutDuration.Should().Be(TimeSpan.FromMinutes(90));
            result.AvgRestTime.Should().Be(TimeSpan.FromSeconds(75));
        }
    
        [Test]
        public async Task ShouldReturnsZeroMetricsGivenEmptySetCollection()
        {
            // Arrange
            var fakeLogs = new[]
            {
                new LogEntryViewModel
                {
                    Id = Guid.NewGuid(),
                    Exercise = new ExerciseViewModel(),
                    Date = new DateTime(2022, 1, 1),
                    Sets = Array.Empty<IExerciseSet>()
                },
            };

            var getWorkoutTimeMetricsRequest = new GetWorkoutTimeMetrics(fakeLogs);

            var handler = new GetWorkoutTimeMetricsHandler();

            // Act
            var result = await handler.Handle(getWorkoutTimeMetricsRequest, CancellationToken.None);

            // Assert
            result.TotalWorkoutTime.Should().Be(TimeSpan.Zero);
            result.TotalRestTime.Should().Be(TimeSpan.Zero);
            result.AvgWorkoutDuration.Should().Be(TimeSpan.Zero);
            result.AvgRestTime.Should().Be(TimeSpan.Zero);
        }
    
        [Test]
        public async Task ShouldIgnoreWorkoutWithOneSet()
        {
            // Arrange
            var fakeLogs = new[]
            {
                new LogEntryViewModel
                {                
                    Id = Guid.NewGuid(),
                    Exercise = new ExerciseViewModel(),
                    Date = new DateTime(2022, 1, 1),
                    Sets = new IExerciseSet[] 
                    { 
                        new CompletedSet { CompletionTime = new DateTime(2022, 1, 1, 1, 0, 0), RestTime = TimeSpan.FromMinutes(1), Weight = 10, Repetitions = 10, Duration = TimeSpan.FromSeconds(10)  },
                    }
                },
            };

            var getWorkoutTimeMetricsRequest = new GetWorkoutTimeMetrics(fakeLogs);

            var handler = new GetWorkoutTimeMetricsHandler();

            // Act
            var result = await handler.Handle(getWorkoutTimeMetricsRequest, CancellationToken.None);

            // Assert
            result.TotalWorkoutTime.Should().Be(TimeSpan.Zero);
            result.TotalRestTime.Should().Be(TimeSpan.Zero);
            result.AvgWorkoutDuration.Should().Be(TimeSpan.Zero);
            result.AvgRestTime.Should().Be(TimeSpan.Zero);
        }
    
        [Test]
        public async Task ShouldIgnoreNoneCompletedSets()
        {
            // Arrange
            var fakeLogs = new[] {
                new LogEntryViewModel
                {
                    Id = Guid.NewGuid(),
                    Exercise = new ExerciseViewModel(),
                    Date = new DateTime(2022, 1, 1),
                    Sets = new IExerciseSet[]
                    {
                        new LegacySet { CompletionTime = new DateTime(2022, 1, 1, 1, 0, 0), RestTime = TimeSpan.FromMinutes(1) },
                        new CompletedSet { CompletionTime = new DateTime(2022, 1, 1, 1, 30, 0), RestTime = TimeSpan.FromMinutes(2), Weight = 10, Repetitions = 10, Duration = TimeSpan.FromSeconds(10)},
                        new ProposedSet(){Weight = 10, Repetitions = 10}
                    }
                }
            };

            var getWorkoutTimeMetricsRequest = new GetWorkoutTimeMetrics(fakeLogs);

            var handler = new GetWorkoutTimeMetricsHandler();

            // Act
            var result = await handler.Handle(getWorkoutTimeMetricsRequest, CancellationToken.None);

            // Assert
            result.TotalWorkoutTime.Should().Be(TimeSpan.FromMinutes(90));
            result.TotalRestTime.Should().Be(TimeSpan.FromMinutes(3));
            result.AvgWorkoutDuration.Should().Be(TimeSpan.FromMinutes(90));
            result.AvgRestTime.Should().Be(TimeSpan.FromSeconds(90));
        }
    }
}