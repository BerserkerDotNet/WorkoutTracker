using System.Globalization;
using WorkoutTracker.Services.Converters;

namespace WorkoutTracker.UnitTests.Converters
{
    [TestFixture]
    public class TimeSpanToTextConverterTests
    {
        private TimeSpanToTextConverter _converter;

        [SetUp]
        public void Setup()
        {
            _converter = new TimeSpanToTextConverter();
        }

        [TestCase(2, 1, 0, 0, "2 day(s)")]
        [TestCase(0, 2, 30, 0, "2.5 hour(s)")]
        [TestCase(0, 0, 2, 30, "2.5 minute(s)")]
        [TestCase(0, 0, 0, 2, "2 second(s)")]
        public void Convert_GivenTimeSpan_ReturnsCorrectText(int days, int hours, int minutes, int seconds, string expected)
        {
            // Arrange
            var timeSpan = new TimeSpan(days, hours, minutes, seconds);

            // Act
            var result = _converter.Convert(timeSpan, null, null, CultureInfo.CurrentCulture);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            // Act
            // Assert
            Assert.Throws<NotImplementedException>(() => _converter.ConvertBack(null, null, null, CultureInfo.CurrentCulture));
        }

        [Test]
        public void Convert_NonTimeSpanInput_ReturnsEmptyString()
        {
            // Arrange
            var nonTimeSpanInput = "NonTimeSpan Input";

            // Act
            var result = _converter.Convert(nonTimeSpanInput, null, null, CultureInfo.CurrentCulture);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }

        [Test]
        public void Convert_NullInput_ReturnsEmptyString()
        {
            // Arrange
            // Act
            var result = _converter.Convert(null, null, null, CultureInfo.CurrentCulture);

            // Assert
            Assert.AreEqual(string.Empty, result);
        }
    }
}