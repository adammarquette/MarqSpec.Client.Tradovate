namespace MarqSpec.Client.Tradovate.Internal;

/// <summary>
/// HTTP helpers that keep Refit routes on the configured <c>/v1</c> host path.
/// </summary>
internal static class TradovateHttp
{
    /// <summary>
    /// Builds a <see cref="HttpClient.BaseAddress"/> that ends with <c>/</c>.
    /// Refit 8 still requires leading-slash routes, so <see cref="TradovateBasePathHandler"/>
    /// re-applies this path after RFC 3986 combination.
    /// </summary>
    /// <param name="restBaseUrl">The configured REST base, with or without a trailing slash.</param>
    /// <returns>An absolute URI ending in <c>/</c>.</returns>
    public static Uri CreateRestBaseAddress(string restBaseUrl)
    {
        string url = restBaseUrl.Trim();
        if (!url.EndsWith('/'))
        {
            url += "/";
        }

        return new Uri(url, UriKind.Absolute);
    }
}
