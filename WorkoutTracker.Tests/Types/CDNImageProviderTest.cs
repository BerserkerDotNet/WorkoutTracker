namespace WorkoutTracker.Tests.Types;

[TestFixture]
public class CDNImageProviderTest
{
    const string CDNHost = "https://workout-tracker-content.azureedge.net/";
    const string BaseImage = $"{CDNHost}images/";
    CDNImageProvider _cdnImageProvider;

    [SetUp]
    public void Setup()
    {
        _cdnImageProvider = new CDNImageProvider(new Uri(BaseImage));
    }

    [Test]
    [TestCase("", BaseImage)]
    [TestCase(null, BaseImage)]
    [TestCase("foo.png", $"{BaseImage}foo.png")]
    [TestCase("exercises/foo.png", $"{BaseImage}exercises/foo.png")]
    [TestCase("exercises\\foo.png", $"{BaseImage}exercises/foo.png")]
    [TestCase("/exercises\\foo.png", $"{BaseImage}exercises/foo.png")]
    [TestCase("\\exercises\\foo.png", $"{BaseImage}exercises/foo.png")]
    public void ShouldGenerateCorrectFullPath(string imagePath, string expected)
    {
        _cdnImageProvider.GetFullPath(imagePath).Should().Be(expected);
    }
}
