using System.Net;
using MarqSpec.Client.Tradovate.Exceptions;
using Refit;

namespace MarqSpec.Client.Tradovate.Internal;

internal static class ApiExceptionMapper
{
    public static Exception Map(ApiException exception)
    {
        if (exception.StatusCode == HttpStatusCode.TooManyRequests)
        {
            TimeSpan? retryAfter = null;
            if (exception.Headers?.RetryAfter?.Delta is { } delta)
            {
                retryAfter = delta;
            }
            else if (exception.Headers?.RetryAfter?.Date is { } date)
            {
                TimeSpan remaining = date - DateTimeOffset.UtcNow;
                if (remaining > TimeSpan.Zero)
                {
                    retryAfter = remaining;
                }
            }

            return new TradovateRateLimitException(
                "Tradovate returned HTTP 429 (rate limited).",
                exception,
                retryAfter);
        }

        return new TradovateApiException(
            $"Tradovate request failed with status code {(int)exception.StatusCode}.",
            (int)exception.StatusCode,
            exception);
    }

    public static bool IsNotFound(ApiException exception)
    {
        return exception.StatusCode == HttpStatusCode.NotFound;
    }
}
