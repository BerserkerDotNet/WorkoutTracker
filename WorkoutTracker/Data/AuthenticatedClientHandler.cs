using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace WorkoutTracker.Data;

public class AuthenticatedClientHandler : DelegatingHandler
{
    private readonly IAccessTokenProvider _accessTokenProvider;

    public AuthenticatedClientHandler(IAccessTokenProvider accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenResult = await _accessTokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            request.Headers.Remove("Authorization");
            request.Headers.Add("Authorization", $"Bearer {token.Value}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
