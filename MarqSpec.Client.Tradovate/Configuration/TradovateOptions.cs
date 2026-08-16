namespace MarqSpec.Client.Tradovate.Configuration;

/// <summary>
/// Configuration options for the Tradovate API client.
/// </summary>
/// <remarks>
/// Host is the environment. <see cref="RestBaseUrl"/> and both socket URLs are required — there is no
/// default and no live fallback. Consumers derive practice-versus-live from the configured host.
/// reference: documentation/INDEX.md (Host-selected environment)
/// </remarks>
public sealed class TradovateOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Tradovate";

    /// <summary>
    /// Gets or sets the Tradovate login name.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Tradovate password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the application identifier sent with <c>accessTokenRequest</c>.
    /// </summary>
    public string? AppId { get; set; }

    /// <summary>
    /// Gets or sets the application version sent with <c>accessTokenRequest</c>.
    /// </summary>
    public string? AppVersion { get; set; }

    /// <summary>
    /// Gets or sets the device identifier sent with <c>accessTokenRequest</c>.
    /// </summary>
    public string? DeviceId { get; set; }

    /// <summary>
    /// Gets or sets the OAuth client id (<c>cid</c>).
    /// </summary>
    public string? Cid { get; set; }

    /// <summary>
    /// Gets or sets the OAuth client secret (<c>sec</c>).
    /// </summary>
    public string? Secret { get; set; }

    /// <summary>
    /// Gets or sets the REST base URL. Required; no default.
    /// </summary>
    /// <remarks>
    /// Demo: <c>https://demo.tradovateapi.com/v1</c>. Live: <c>https://live.tradovateapi.com/v1</c>.
    /// A trailing slash is optional. Refit requires leading-slash routes, so the client re-applies this path
    /// after URI combination so <c>/v1</c> is not dropped.
    /// </remarks>
    public string RestBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the trading/account WebSocket URL. Required; no default.
    /// </summary>
    public string TradingSocketUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the market-data WebSocket URL. Required; no default.
    /// </summary>
    public string MarketDataSocketUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether TLS certificates are validated. Default is <see langword="true"/>.
    /// </summary>
    public bool ValidateSslCertificates { get; set; } = true;

    /// <summary>
    /// Gets or sets how far ahead of token expiry renewal should run. Default is 15 minutes.
    /// </summary>
    public TimeSpan TokenRenewalLeadTime { get; set; } = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Gets or sets retry options for idempotent REST calls.
    /// </summary>
    public RetryOptions Retry { get; set; } = new();

    /// <summary>
    /// Gets or sets WebSocket heartbeat and stall options.
    /// </summary>
    public WebSocketOptions WebSocket { get; set; } = new();

    /// <summary>
    /// Validates that required host and credential fields are present.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when a required field is missing. The message never includes credential values.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(RestBaseUrl))
        {
            throw new InvalidOperationException("RestBaseUrl is required. Configure the Tradovate host explicitly; there is no default and no live fallback.");
        }

        if (string.IsNullOrWhiteSpace(TradingSocketUrl))
        {
            throw new InvalidOperationException("TradingSocketUrl is required. Configure the trading socket host explicitly; there is no default and no live fallback.");
        }

        if (string.IsNullOrWhiteSpace(MarketDataSocketUrl))
        {
            throw new InvalidOperationException("MarketDataSocketUrl is required. Configure the market-data socket host explicitly; there is no default and no live fallback.");
        }

        if (string.IsNullOrWhiteSpace(Username))
        {
            throw new InvalidOperationException("Username is required. Configure it via environment variable TRADOVATE_USERNAME or the Tradovate options section.");
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            throw new InvalidOperationException("Password is required. Configure it via environment variable TRADOVATE_PASSWORD or the Tradovate options section.");
        }
    }
}
