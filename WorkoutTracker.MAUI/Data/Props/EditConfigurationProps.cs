using Microsoft.AspNetCore.Components;
using WorkoutTracker.MAUI.Android;

namespace WorkoutTracker.MAUI.Data.Props
{
    public class EditConfigurationProps
    {
        public string Url { get; set; }

        public string Secret { get; set; }

        public EventCallback<EndpointConfiguration> Save { get; set; }
    }
}
