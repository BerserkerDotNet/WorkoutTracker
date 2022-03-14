using Microsoft.AspNetCore.Components;

namespace WorkoutTracker.Data.Props
{
    public class EditConfigurationProps
    {
        public string Url { get; set; }

        public string Secret { get; set; }

        public EventCallback<EndpointConfiguration> Save { get; set; }
    }
}
