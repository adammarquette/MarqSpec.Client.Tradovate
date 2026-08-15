namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// One OHLCV bar from the market-data socket (<c>md/getChart</c>).
/// </summary>
public sealed record ChartBar
{
    /// <summary>Gets the bar timestamp (UTC).</summary>
    public required DateTimeOffset Timestamp { get; init; }

    /// <summary>Gets the open price.</summary>
    public required decimal Open { get; init; }

    /// <summary>Gets the high price.</summary>
    public required decimal High { get; init; }

    /// <summary>Gets the low price.</summary>
    public required decimal Low { get; init; }

    /// <summary>Gets the close price.</summary>
    public required decimal Close { get; init; }

    /// <summary>Gets the volume. Null when the socket omitted it.</summary>
    public decimal? Volume { get; init; }

    /// <summary>Gets the up volume, when present.</summary>
    public decimal? UpVolume { get; init; }

    /// <summary>Gets the down volume, when present.</summary>
    public decimal? DownVolume { get; init; }
}
