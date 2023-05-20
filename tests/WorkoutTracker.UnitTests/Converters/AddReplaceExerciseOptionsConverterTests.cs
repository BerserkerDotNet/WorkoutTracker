using FluentAssertions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using System.Globalization;
using WorkoutTracker.Models.Presentation;
using WorkoutTracker.Services.Converters;
using WorkoutTracker.Services.ViewModels;

namespace WorkoutTracker.UnitTests.Converters;

[TestFixture]
public class AddReplaceExerciseOptionsConverterTests
{
    private AddReplaceExerciseOptionsConverter _converter;
    
    [SetUp]
    public void Init()
    {
        _converter = new AddReplaceExerciseOptionsConverter();
    }

    [Test]
    public void CreatesOptionsObjectWithWarmup()
    {
        var exercise = new ExerciseViewModel();

        var result = _converter.Convert(new object[] { exercise, true }, typeof(object), null, CultureInfo.CurrentCulture);

        result.Should().BeOfType<AddReplaceExerciseOptions>();
        result.Should().NotBeNull();
        result.As<AddReplaceExerciseOptions>().Exercise.Should().Be(exercise);
        result.As<AddReplaceExerciseOptions>().IncludeWarmup.Should().BeTrue();
    }
    
    [Test]
    public void CreatesOptionsObjectWithoutWarmup()
    {
        var exercise = new ExerciseViewModel();

        var result = _converter.Convert(new object[] { exercise, false }, typeof(object), null, CultureInfo.CurrentCulture);

        result.Should().BeOfType<AddReplaceExerciseOptions>();
        result.Should().NotBeNull();
        result.As<AddReplaceExerciseOptions>().Exercise.Should().Be(exercise);
        result.As<AddReplaceExerciseOptions>().IncludeWarmup.Should().BeFalse();
    }
    
    [Test]
    public void ReturnsNullIfExerciseIsNull()
    {
        var result = _converter.Convert(new object[] { null, false }, typeof(object), null, CultureInfo.CurrentCulture);

        result.Should().BeNull();
    }
    
    [Test]
    public void ReturnsNullIfWarmupIsNull()
    {
        var exercise = new ExerciseViewModel();
        var result = _converter.Convert(new object[] { exercise, null }, typeof(object), null, CultureInfo.CurrentCulture);

        result.Should().BeNull();
    }
    
    [Test]
    public void ReturnsNullIfOneItemIsPassed()
    {
        var exercise = new ExerciseViewModel();
        var result = _converter.Convert(new object[] { exercise }, typeof(object), null, CultureInfo.CurrentCulture);

        result.Should().BeNull();
    }
    
    [Test]
    public void ReturnsNullIfNoItemsPassed()
    {
        var result = _converter.Convert(Array.Empty<object>(), typeof(object), null, CultureInfo.CurrentCulture);

        result.Should().BeNull();
    }

    [Test]
    public void ConvertBackIsNotSupported()
    {
        _converter.Invoking(c => c.ConvertBack(null, Type.EmptyTypes, null, CultureInfo.CurrentCulture))
            .Should().Throw<NotSupportedException>();
    }
}