using Bunit;
using MudBlazor;
using MudBlazor.Services;
using NUnit.Framework;

namespace WorkoutTracker.Tests.Components;

public abstract class BaseComponentTest : TestContextWrapper
{
    [SetUp]
    public void Setup()
    {
        TestContext = new Bunit.TestContext();
        TestContext.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 2000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Outlined;
        });
        Initialize();
    }

    public virtual void Initialize()
    {
    }

    [TearDown]
    public void TearDown() => TestContext?.Dispose();
}
