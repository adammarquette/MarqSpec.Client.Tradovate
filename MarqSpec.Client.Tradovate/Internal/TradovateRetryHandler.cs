using System.Net;
using MarqSpec.Client.Tradovate.Configuration;

namespace MarqSpec.Client.Tradovate.Internal;

/// <summary>
/// Retries reads, cancel, and modify on 429 / 5xx / transport. Never retries place or liquidate.
/// </summary>
internal sealed class TradovateRetryHandler : DelegatingHandler
{
    private readonly RetryOptions _options;

    public TradovateRetryHandler(RetryOptions options)
    {
        _options = options;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (CommandEndpoints.IsNeverRetried(request.RequestUri))
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }

        Exception? lastException = null;
        HttpResponseMessage? lastResponse = null;
        int maxAttempts = Math.Max(1, _options.MaxRetryAttempts + 1);

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                lastResponse = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                if (!ShouldRetry(lastResponse.StatusCode) || attempt == maxAttempts)
                {
                    return lastResponse;
                }

                TimeSpan delay = ResolveDelay(lastResponse, attempt);
                lastResponse.Dispose();
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException ex) when (attempt < maxAttempts)
            {
                lastException = ex;
                await Task.Delay(ComputeBackoff(attempt), cancellationToken).ConfigureAwait(false);
            }
        }

        if (lastResponse is not null)
        {
            return lastResponse;
        }

        throw lastException ?? new HttpRequestException("The request failed after retries.");
    }

    private static bool ShouldRetry(HttpStatusCode statusCode)
    {
        return statusCode == HttpStatusCode.TooManyRequests
            || (int)statusCode >= 500;
    }

    private TimeSpan ResolveDelay(HttpResponseMessage response, int attempt)
    {
        if (response.Headers.RetryAfter is { } retryAfter)
        {
            TimeSpan? delay = retryAfter.Delta
                ?? (retryAfter.Date.HasValue ? retryAfter.Date.Value - DateTimeOffset.UtcNow : null);
            if (delay is { } value && value > TimeSpan.Zero)
            {
                return value < _options.MaxDelay ? value : _options.MaxDelay;
            }
        }

        return ComputeBackoff(attempt);
    }

    private TimeSpan ComputeBackoff(int attempt)
    {
        double factor = Math.Pow(2, attempt - 1);
        TimeSpan delay = TimeSpan.FromMilliseconds(_options.BaseDelay.TotalMilliseconds * factor);
        return delay < _options.MaxDelay ? delay : _options.MaxDelay;
    }
}
