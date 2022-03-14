using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WorkoutTracker;
using WorkoutTracker.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddWorkoutTracker();

builder.Services.AddMsalAuthentication(options =>
{
    var configuration = builder.Configuration;

    var authentication = options.ProviderOptions.Authentication;
    authentication.Authority = "https://login.microsoftonline.com/common";

    authentication.ClientId = configuration["clientId"];
    authentication.PostLogoutRedirectUri = "https://localhost:7210";
    options.ProviderOptions.DefaultAccessTokenScopes.Add($"{authentication.ClientId}/user_impersonation");
});

await builder.Build().RunAsync();
