using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Exceptions;

namespace MarqSpec.Client.Tradovate.Internal;

/// <summary>
/// Throws when a Tradovate command hid failure inside a 200 payload.
/// </summary>
internal static class CommandResultGuard
{
    public static void EnsureSuccess(
        FailureReason? failureReason,
        string? failureText,
        long? orderId,
        string operation,
        bool requireOrderId = false)
    {
        if (failureReason is { } reason && reason != FailureReason.Success)
        {
            string detail = string.IsNullOrWhiteSpace(failureText) ? reason.ToString() : $"{reason}: {failureText}";
            throw new TradovateApiException(
                $"{operation} was rejected ({detail}).",
                statusCode: 200,
                failureReason: reason);
        }

        if (requireOrderId && orderId is null)
        {
            throw new TradovateApiException(
                $"{operation} returned no order id and no success failureReason.",
                statusCode: 200,
                failureReason: failureReason);
        }
    }
}
