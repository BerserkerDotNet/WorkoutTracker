using BlazorStorage.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WorkoutTracker.Extensions;
using WorkoutTracker.Models.Mappings;
using WorkoutTracker.Web;

Mappings.Configure();

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddStorage();
builder.Services.AddWorkoutTracker(builder.Configuration, cfg =>
{
    cfg.WithCacheService<LocalStorageCacheService>();
});

builder.Services.AddMsalAuthentication(options =>
{
    var configuration = builder.Configuration;

    var authentication = options.ProviderOptions.Authentication;
    authentication.Authority = "https://login.microsoftonline.com/563fec59-dcd7-47a5-a09a-6d44ab624093";

    authentication.ClientId = configuration["clientId"];
    authentication.PostLogoutRedirectUri = "https://localhost:7210";
    options.ProviderOptions.DefaultAccessTokenScopes.Add($"{authentication.ClientId}/access_as_user");
});

await builder.Build().RunAsync();
