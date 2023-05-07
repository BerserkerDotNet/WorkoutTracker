namespace WorkoutTracker.Services.Interfaces;

public interface INavigation
{
    Task GoTo(string path, Dictionary<string, object> parameters);

    Task GoBack();
}