using BlazorState.Redux.Blazor;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Android;

namespace WorkoutTracker.MAUI.Components.Connected
{
    public class EditConfigurationConnected : ConnectedComponent<EditConfiguration, RootState, EditConfigurationProps>
    {
        private EndpointConfiguration configuration = new EndpointConfiguration("", "");

        [Inject]
        public IConfigurationService ConfigurationService { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        protected override async Task Init(IStore<RootState> store)
        {
            if (ConfigurationService.IsConfigurationAvailable()) 
            {
                configuration = await ConfigurationService.GetEndpointConfigurationAsync();
            }
        }

        protected override void MapDispatchToProps(IStore<RootState> store, EditConfigurationProps props)
        {
            props.Save = EventCallback.Factory.Create<EndpointConfiguration>(this, async config =>
            {
                await ConfigurationService.SaveEndpointConfiguration(config);
                Navigation.NavigateTo($"/");
            });
        }

        protected override void MapStateToProps(RootState state, EditConfigurationProps props)
        {
            props.Url = configuration.Url;
            props.Secret = configuration.Secret;
        }
    }
}
