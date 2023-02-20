using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Maui.ApplicationModel;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI.Services.Data;

public class AuthenticationService
{
    private readonly IPublicClientApplication _authenticationClient;
    private string[] _scopes = new[] { "234eb3af-aa5d-45ad-a950-c69d7f3e0c8d/access_as_user" };

    public AuthenticationService(IConfiguration config)
    {
        _authenticationClient = PublicClientApplicationBuilder.Create(config["clientId"])
            .WithAuthority("https://login.microsoftonline.com/563fec59-dcd7-47a5-a09a-6d44ab624093/")
            .WithRedirectUri(config["redirectUrl"])
            .WithParentActivityOrWindow(() => Platform.CurrentActivity)
            .Build();
    }

    public virtual async Task<string> GetTokenAsync()
    {
        var accounts = await _authenticationClient.GetAccountsAsync();
        if (accounts.Any())
        {
            try
            {
                var silentResult = await _authenticationClient
                    .AcquireTokenSilent(_scopes, accounts.First())
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                return silentResult.AccessToken;
            }
            catch
            {
            }
        }

        var result = await _authenticationClient
            .AcquireTokenInteractive(_scopes)
            .ExecuteAsync()
            .ConfigureAwait(false);

        return result.AccessToken;
    }
}