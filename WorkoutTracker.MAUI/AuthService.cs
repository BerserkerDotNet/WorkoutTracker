using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Maui.Essentials;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace WorkoutTracker.MAUI;

public class AuthService : AuthenticationStateProvider, IRemoteAuthenticationService<RemoteAuthenticationState>, IAccessTokenProvider
{
    private readonly IPublicClientApplication _authenticationClient;
    private readonly ICacheService _cacheService;
    private AccessToken _idToken;
    private ClaimsPrincipal _principal;
    private RemoteAuthenticationStatus? _lastStatus;

    public AuthService(ICacheService cacheService, IConfiguration config)
    {
        _authenticationClient = PublicClientApplicationBuilder.Create(config["clientId"])
            .WithRedirectUri(config["redirectUrl"])
            .WithParentActivityOrWindow(() => Platform.CurrentActivity)
            .Build();
        _cacheService = cacheService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _idToken = await _cacheService.GetToken();
        var claimsIdentity = new ClaimsIdentity();
        if (!string.IsNullOrEmpty(_idToken?.Value) && _idToken.Expires > DateTime.Now)
        {
            var handler = new JwtSecurityTokenHandler();
            var data = handler.ReadJwtToken(_idToken.Value);
            if (data is object)
            {
                claimsIdentity = GetIdentityFrom(data.Claims);
            }
        }

        _principal = new ClaimsPrincipal(claimsIdentity);
        var authState = new AuthenticationState(_principal);

        return authState;
    }

    public async Task<RemoteAuthenticationResult<RemoteAuthenticationState>> SignInAsync(RemoteAuthenticationContext<RemoteAuthenticationState> context)
    {
        try
        {
            var tokenExpired = _idToken?.Expires <= DateTime.Now;

            if (_idToken is object && !tokenExpired && _lastStatus == RemoteAuthenticationStatus.Success)
            {
                return new RemoteAuthenticationResult<RemoteAuthenticationState>
                {
                    Status = RemoteAuthenticationStatus.Redirect,
                    State = context.State
                };
            }

            if (_idToken is null || tokenExpired)
            {
                var result = await _authenticationClient
                    .AcquireTokenInteractive(new[] { "openid", "offline_access" })
                    .WithPrompt(Prompt.ForceLogin)
                    .ExecuteAsync()
                    .ConfigureAwait(false);
                if (result.IdToken is null)
                {
                    throw new MsalClientException("Token is null");
                }

                _idToken = new AccessToken { Value = result.IdToken, Expires = result.ExpiresOn, GrantedScopes = result.Scopes.ToList() };
                _principal = new ClaimsPrincipal(GetIdentityFrom(result.ClaimsPrincipal.Claims));
                await _cacheService.SaveToken(_idToken);
            }

            _lastStatus = RemoteAuthenticationStatus.Success;

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_principal)));

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

    public ValueTask<AccessTokenResult> RequestAccessToken()
    {
        return new ValueTask<AccessTokenResult>(Task.FromResult(new AccessTokenResult(AccessTokenResultStatus.Success, _idToken, "/")));
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
