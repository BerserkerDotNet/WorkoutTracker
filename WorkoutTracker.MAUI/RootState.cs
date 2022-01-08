using Microsoft.AspNetCore.Components;
using WorkoutTracker.MAUI.Data.Reducers;

namespace WorkoutTracker.MAUI
{
	public class RootState
    {
        public ExercisesState Exercises { get; set; }
    }

    [EventHandler("onswiped-right", typeof(SwipedEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
    [EventHandler("onswiped-left", typeof(SwipedEventArgs), enableStopPropagation: true, enablePreventDefault: true)]
    public static class EventHandlers
    {
        // This static class doesn't need to contain any members. It's just a place where we can put
        // [EventHandler] attributes to configure event types on the Razor compiler. This affects the
        // compiler output as well as code completions in the editor.
    }

    public class SwipedEventArgs : EventArgs
    {
        public object Target { get; set; }

        public string Direction { get; set; }
    }

    public record SetDetail(int Duration, int Repetitions, int Rating, string Notes);
}
