namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate trade date (year / month / day).
/// </summary>
public sealed record TradeDate
{
    /// <summary>Gets the year.</summary>
    public required int Year { get; init; }

    /// <summary>Gets the month.</summary>
    public required int Month { get; init; }

    /// <summary>Gets the day.</summary>
    public required int Day { get; init; }
}
