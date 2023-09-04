using FluentAssertions;
using NSubstitute.ExceptionExtensions;
using WorkoutTracker.Models.Entities;
using WorkoutTracker.Services.Converters;

namespace WorkoutTracker.UnitTests.Converters
{
    [TestFixture]
    public class SetToColorConverterTests
    {
        private SetToColorConverter _converter;

        [SetUp]
        public void SetUp()
        {
            _converter = new SetToColorConverter();
        }

        [Test]
        public void ConvertCompletedSetColorReturnsLightGreen()
        {
            // Arrange
            var completedSet = new CompletedSet
            {
                Weight = 10,
                Duration = TimeSpan.Zero,
                Repetitions = 15,
                CompletionTime = DateTime.UtcNow,
                RestTime = TimeSpan.Zero
            };

            // Act
            var result = _converter.Convert(completedSet, null, null, null);

            // Assert
            result.Should().Be(Colors.LightGreen);
        }

        [Test]
        public void ConvertInProgressSetColorReturnsLightPink()
        {
            // Arrange
            var inProgressSet = new InProgressSet
            {
                Repetitions = 15,
                Weight = 10,
                RestTime = TimeSpan.Zero
            };

            // Act
            var result = _converter.Convert(inProgressSet, null, null, null);

            // Assert
            result.Should().Be(Colors.LightPink);
        }

        [Test]
        public void ConvertLegacySetColorReturnsLightGreen()
        {
            // Arrange
            var legacySet = new LegacySet();

            // Act
            var result = _converter.Convert(legacySet, null, null, null);

            // Assert
            result.Should().Be(Colors.LightGreen);
        }

        [Test]
        public void ConvertProposedSetReturnsLightSkyBlue()
        {
            // Arrange
            var proposedSet = new ProposedSet()
            {
                Repetitions = 15,
                Weight = 15
            };

            // Act
            var result = _converter.Convert(proposedSet, null, null, null);

            // Assert
            result.Should().Be(Colors.LightSkyBlue);
        }

        [Test]
        public void ConvertNullReturnsTransparent()
        {
            // Act
            var result = _converter.Convert(null, null, null, null);

            // Assert
            result.Should().Be(Colors.Transparent);
        }

        [Test]
        public void ConvertUnknownTypeReturnsTransparent()
        {
            // Arrange
            var unknownType = new Object();

            // Act
            var result = _converter.Convert(unknownType, null, null, null);

            // Assert
            result.Should().Be(Colors.Transparent);
        }
        
        [Test]
        public void ConvertBackThrowsNotImplementedException()
        {
            // Act & Assert
            _converter.Invoking(c => c.ConvertBack(null, null, null, null))
                .Should().Throw<NotImplementedException>();
        }
    }
}