using INavigation = WorkoutTracker.Services.Interfaces.INavigation;

namespace WorkoutTracker.Services;

public class ShellNavigation: INavigation
{
    public Task GoTo(string path, Dictionary<string, object> parameters)
    {
        return Shell.Current.GoToAsync(path, parameters);
    }

    public Task GoBack()
    {
        return Shell.Current.GoToAsync("..");
    }
}