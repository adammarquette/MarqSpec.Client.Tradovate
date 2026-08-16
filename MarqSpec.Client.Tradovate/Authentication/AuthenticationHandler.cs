using System.Net.Http.Headers;

namespace MarqSpec.Client.Tradovate.Authentication;

/// <summary>
/// Adds a Bearer trading token to REST calls. Never logs the token.
/// </summary>
internal sealed class AuthenticationHandler : DelegatingHandler
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationHandler(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string token = await _authenticationService.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
