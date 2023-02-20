using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WorkoutTracker.MAUI.Services.Data;

namespace WorkoutTracker.MAUI.Services;

public class AuthenticatedClientHandler : DelegatingHandler
{
    private readonly AuthenticationService _accessTokenProvider;

    public AuthenticatedClientHandler(AuthenticationService accessTokenProvider)
    {
        _accessTokenProvider = accessTokenProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _accessTokenProvider.GetTokenAsync();
        if (token is not null)
        {
            request.Headers.Remove("Authorization");
            request.Headers.Add("Authorization", $"Bearer {token}");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
