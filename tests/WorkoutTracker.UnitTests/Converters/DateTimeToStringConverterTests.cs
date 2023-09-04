using FluentAssertions;
using System.Globalization;
using WorkoutTracker.Services.Converters;

[TestFixture]
class DateTimeToStringConverterTests
{
    private DateTimeToStringConverter _converter;

    // Execute before each test is run
    [SetUp]
    public void Setup()
    {
        _converter = new DateTimeToStringConverter();
    }

    [Test]
    public void ConvertWithDateTimeValueReturnsCorrectFormat()
    {
        var input = new DateTime(2022, 6, 12);
        var expectedOutput = "Sunday, 06/12/2022";
        
        var result = _converter.Convert(input, null, null, CultureInfo.CurrentCulture);
        
        result.Should().Be(expectedOutput);
    }

    [Test]
    public void ConvertWithNonDateTimeValueReturnsToString()
    {
        var input = 123;
        var expectedOutput = "123";
        
        var result = _converter.Convert(input, null, null, CultureInfo.CurrentCulture);
        
        result.Should().Be(expectedOutput);
    }

    [Test]
    public void ConvertWithNullValueReturnsNull()
    {
        var result = _converter.Convert(null, null, null, CultureInfo.CurrentCulture);
        
        result.Should().Be("N/A");
    }

    // As ConvertBack is not implemented, it should always throw a NotImplementedException
    [Test]
    public void ConvertBackThrowsNotImplementedException()
    {
      Action action = () => _converter.ConvertBack(null, null, null, CultureInfo.CurrentCulture);
      action.Should().Throw<NotImplementedException>();
    }
}