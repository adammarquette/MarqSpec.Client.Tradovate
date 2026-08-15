using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Api.Rest;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace MarqSpec.Client.Tradovate.Authentication;

/// <summary>
/// Thread-safe dual-token acquire/renew. Tokens live ~90 minutes; renewal starts
/// <see cref="TradovateOptions.TokenRenewalLeadTime"/> before expiry.
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly ITradovateAuthApi _authApi;
    private readonly TradovateOptions _options;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private string? _accessToken;
    private string? _mdAccessToken;
    private DateTimeOffset _expiresAt = DateTimeOffset.MinValue;
    private long? _userId;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
    /// </summary>
    /// <param name="authApi">The unauthenticated auth API.</param>
    /// <param name="options">The Tradovate options.</param>
    /// <param name="logger">The logger. Must never receive credentials or tokens.</param>
    internal AuthenticationService(
        ITradovateAuthApi authApi,
        IOptions<TradovateOptions> options,
        ILogger<AuthenticationService> logger)
    {
        _authApi = authApi;
        _options = options.Value;
        _logger = logger;
        _options.Validate();
    }

    /// <inheritdoc />
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        await EnsureTokensAsync(cancellationToken).ConfigureAwait(false);
        return _accessToken!;
    }

    /// <inheritdoc />
    public async Task<string> GetMarketDataAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        await EnsureTokensAsync(cancellationToken).ConfigureAwait(false);
        return _mdAccessToken ?? _accessToken!;
    }

    /// <inheritdoc />
    public async Task<long?> GetUserIdAsync(CancellationToken cancellationToken = default)
    {
        await EnsureTokensAsync(cancellationToken).ConfigureAwait(false);
        return _userId;
    }

    /// <inheritdoc />
    public async Task RefreshTokensAsync(CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await RefreshUnlockedAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task EnsureTokensAsync(CancellationToken cancellationToken)
    {
        if (HasFreshTokens())
        {
            return;
        }

        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (HasFreshTokens())
            {
                return;
            }

            await RefreshUnlockedAsync(cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    private bool HasFreshTokens()
    {
        return !string.IsNullOrEmpty(_accessToken)
            && _expiresAt > DateTimeOffset.UtcNow + _options.TokenRenewalLeadTime;
    }

    private async Task RefreshUnlockedAsync(CancellationToken cancellationToken)
    {
        try
        {
            AccessTokenResponse response;
            if (!string.IsNullOrEmpty(_accessToken))
            {
                _logger.LogDebug("Renewing Tradovate access tokens");
                response = await _authApi.RenewAccessTokenAsync("Bearer " + _accessToken, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                _logger.LogDebug("Requesting Tradovate access tokens");
                response = await _authApi.RequestAccessTokenAsync(CreateRequest(), cancellationToken).ConfigureAwait(false);
            }

            ApplyResponse(response);
        }
        catch (ApiException ex)
        {
            _logger.LogError("Tradovate authentication failed with status {StatusCode}", (int)ex.StatusCode);
            ClearTokens();
            throw new AuthenticationException(
                $"Authentication failed with status code {(int)ex.StatusCode}. Verify the configured credentials.",
                ex);
        }
        catch (AuthenticationException)
        {
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error during Tradovate authentication");
            throw new AuthenticationException("Network error during authentication.", ex);
        }
    }

    private AccessTokenRequest CreateRequest()
    {
        return new AccessTokenRequest
        {
            Name = _options.Username,
            Password = _options.Password,
            AppId = _options.AppId,
            AppVersion = _options.AppVersion,
            DeviceId = _options.DeviceId,
            Cid = _options.Cid,
            Sec = _options.Secret,
        };
    }

    private void ApplyResponse(AccessTokenResponse response)
    {
        if (!string.IsNullOrWhiteSpace(response.ErrorText) || string.IsNullOrWhiteSpace(response.AccessToken))
        {
            ClearTokens();
            throw new AuthenticationException("Authentication failed. The server rejected the token request.");
        }

        _accessToken = response.AccessToken;
        _mdAccessToken = string.IsNullOrWhiteSpace(response.MdAccessToken) ? null : response.MdAccessToken;
        _userId = response.UserId;
        _expiresAt = response.ExpirationTime ?? DateTimeOffset.UtcNow.AddMinutes(90);
        _logger.LogInformation("Acquired Tradovate access tokens; they expire at {ExpirationTime:o}", _expiresAt);
    }

    private void ClearTokens()
    {
        _accessToken = null;
        _mdAccessToken = null;
        _userId = null;
        _expiresAt = DateTimeOffset.MinValue;
    }
}
