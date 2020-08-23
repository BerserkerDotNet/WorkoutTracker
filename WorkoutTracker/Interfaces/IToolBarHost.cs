using System;

namespace WorkoutTracker.Interfaces
{
    public interface IToolBarHost
    {
        event EventHandler OnFabClicked;

        bool FabButtonVisible { get; set; }
    }
}