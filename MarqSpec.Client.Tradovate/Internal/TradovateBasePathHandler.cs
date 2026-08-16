namespace MarqSpec.Client.Tradovate.Internal;

/// <summary>
/// Re-applies the configured REST path (e.g. <c>/v1</c>) after Refit builds a leading-slash route.
/// Refit 8 requires <c>/order/placeorder</c>, which RFC 3986 treats as an absolute-path reference and
/// would otherwise drop <c>/v1</c> from <c>https://demo.tradovateapi.com/v1</c>.
/// </summary>
internal sealed class TradovateBasePathHandler : DelegatingHandler
{
    private readonly string _basePath;

    public TradovateBasePathHandler(Uri restBaseAddress)
    {
        _basePath = restBaseAddress.AbsolutePath.TrimEnd('/');
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (_basePath.Length > 0 && request.RequestUri is { IsAbsoluteUri: true } uri)
        {
            request.RequestUri = ApplyBasePath(uri, _basePath);
        }

        return base.SendAsync(request, cancellationToken);
    }

    internal static Uri ApplyBasePath(Uri requestUri, string basePath)
    {
        string path = requestUri.AbsolutePath;
        string remainder = path.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)
            ? path[basePath.Length..]
            : path;
        remainder = "/" + remainder.TrimStart('/');

        return new UriBuilder(requestUri)
        {
            Path = basePath + remainder,
        }.Uri;
    }
}
