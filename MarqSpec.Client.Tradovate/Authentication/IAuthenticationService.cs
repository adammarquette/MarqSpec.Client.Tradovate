namespace MarqSpec.Client.Tradovate.Authentication;

/// <summary>
/// Acquires and renews Tradovate's dual tokens (<c>accessToken</c> and <c>mdAccessToken</c>).
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Gets a valid trading/account access token, renewing when inside the lead-time window.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The access token. Never log this value.</returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a valid market-data access token, falling back to the trading token when the server omitted <c>mdAccessToken</c>.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The market-data token. Never log this value.</returns>
    Task<string> GetMarketDataAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the authenticated user id from the last successful token response, acquiring tokens if needed.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The user id, or <see langword="null"/> when the server omitted it.</returns>
    Task<long?> GetUserIdAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Forces a token acquire or renew.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task RefreshTokensAsync(CancellationToken cancellationToken = default);
}
