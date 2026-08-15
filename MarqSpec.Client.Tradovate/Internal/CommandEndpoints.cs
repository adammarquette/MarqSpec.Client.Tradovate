namespace MarqSpec.Client.Tradovate.Internal;

/// <summary>
/// Paths that must never be auto-retried.
/// </summary>
internal static class CommandEndpoints
{
    public static bool IsNeverRetried(Uri? requestUri)
    {
        if (requestUri is null)
        {
            return false;
        }

        return IsNeverRetried(requestUri.AbsolutePath);
    }

    public static bool IsNeverRetried(string path)
    {
        ReadOnlySpan<char> absolute = path.AsSpan();
        if (absolute.EndsWith("/order/placeorder", StringComparison.OrdinalIgnoreCase)
            || absolute.EndsWith("/order/placeoso", StringComparison.OrdinalIgnoreCase)
            || absolute.EndsWith("/order/placeoco", StringComparison.OrdinalIgnoreCase)
            || absolute.EndsWith("/order/liquidateposition", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }
}
