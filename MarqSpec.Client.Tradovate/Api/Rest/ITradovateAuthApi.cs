using MarqSpec.Client.Tradovate.Api.Models;
using Refit;

namespace MarqSpec.Client.Tradovate.Api.Rest;

/// <summary>
/// Unauthenticated / token-renewal auth endpoints.
/// </summary>
internal interface ITradovateAuthApi
{
    [Post("/auth/accesstokenrequest")]
    Task<AccessTokenResponse> RequestAccessTokenAsync([Body] AccessTokenRequest request, CancellationToken cancellationToken = default);

    [Get("/auth/renewaccesstoken")]
    Task<AccessTokenResponse> RenewAccessTokenAsync([Header("Authorization")] string authorization, CancellationToken cancellationToken = default);
}
