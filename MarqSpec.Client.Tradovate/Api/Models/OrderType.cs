namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate order type.
/// </summary>
public enum OrderType
{
    /// <summary>Limit.</summary>
    Limit = 0,

    /// <summary>Market-if-touched.</summary>
    MIT,

    /// <summary>Market.</summary>
    Market,

    /// <summary>Quote-trigger stop.</summary>
    QTS,

    /// <summary>Stop.</summary>
    Stop,

    /// <summary>Stop-limit.</summary>
    StopLimit,

    /// <summary>Trailing stop.</summary>
    TrailingStop,

    /// <summary>Trailing stop-limit.</summary>
    TrailingStopLimit,
}
