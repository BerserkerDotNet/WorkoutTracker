using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Maui.ApplicationModel;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI;

public class AuthService : AuthenticationStateProvider, IRemoteAuthenticationService<RemoteAuthenticationState>, IAccessTokenProvider
{
    private readonly IPublicClientApplication _authenticationClient;
    private RemoteAuthenticationStatus? _lastStatus;
    private AuthenticationState _currentAuthState;
    private string[] _scopes = new[] { "234eb3af-aa5d-45ad-a950-c69d7f3e0c8d/access_as_user" };

    public AuthService(IConfiguration config)
    {
        var claimsIdentity = new ClaimsIdentity();
        var principal = new ClaimsPrincipal(claimsIdentity);
        _currentAuthState = new AuthenticationState(principal);

        _authenticationClient = PublicClientApplicationBuilder.Create(config["clientId"])
            .WithAuthority("https://login.microsoftonline.com/563fec59-dcd7-47a5-a09a-6d44ab624093/")
            .WithRedirectUri(config["redirectUrl"])
            .WithParentActivityOrWindow(() => Platform.CurrentActivity)
            .Build();
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(_currentAuthState);
    }

    public async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
    {
        try
        {
            if (_lastStatus == RemoteAuthenticationStatus.Success)
            {
                return new RemoteAuthenticationResult<RemoteAuthenticationState>
                {
                    Status = RemoteAuthenticationStatus.Redirect,
                    State = context.State
                };
            }

            var accounts = await _authenticationClient.GetAccountsAsync();

            AuthenticationResult result = null;

            if (accounts.Any())
            {
                result = await _authenticationClient
                    .AcquireTokenSilent(_scopes, accounts.First())
                    .ExecuteAsync()
                    .ConfigureAwait(false);
            }
            else
            {
                result = await _authenticationClient
                    .AcquireTokenInteractive(_scopes)
                    .WithPrompt(Prompt.ForceLogin)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                if (result.IdToken is null)
                {
                    throw new MsalClientException("Token is null");
                }
            }

            _lastStatus = RemoteAuthenticationStatus.Success;
            var principal = new ClaimsPrincipal(GetIdentityFrom(result.ClaimsPrincipal.Claims));
            _currentAuthState = new AuthenticationState(principal);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

            return new RemoteAuthenticationResult<RemoteAuthenticationState>
            {
                Status = _lastStatus.Value,
                State = context.State
            };

        }
        catch (MsalClientException ex)
        {
            return new RemoteAuthenticationResult<RemoteAuthenticationState>
            {
                ErrorMessage = ex.Message,
                Status = RemoteAuthenticationStatus.Failure,
                State = context.State
            };
        }
    }

    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        var accounts = await _authenticationClient.GetAccountsAsync();
        var result = await _authenticationClient
            .AcquireTokenSilent(_scopes, accounts.First())
            .ExecuteAsync()
            .ConfigureAwait(false);
        var token = new AccessToken { Value = result.AccessToken, Expires = result.ExpiresOn, GrantedScopes = result.Scopes.ToList() };
        return new AccessTokenResult(AccessTokenResultStatus.Success, token, "/");
    }

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
    {
        return RequestAccessToken();
    }

    public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context) => throw new System.NotImplementedException();

    public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> CompleteSignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context) => throw new System.NotImplementedException();

    public Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignOutAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context) => throw new System.NotImplementedException();

    private ClaimsIdentity GetIdentityFrom(IEnumerable<Claim> claims)
    {
        return new ClaimsIdentity(claims, authenticationType: "Bearer", nameType: "name", null);
    }
}

internal class EmptyOptions
{
}
