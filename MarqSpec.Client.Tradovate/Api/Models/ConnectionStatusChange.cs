namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Raised when one of the two Tradovate sockets changes connection state.
/// </summary>
public sealed record ConnectionStatusChange
{
    /// <summary>Gets a value indicating whether this update is for the trading socket.</summary>
    public required bool IsTradingSocket { get; init; }

    /// <summary>Gets the previous state.</summary>
    public required ConnectionState Previous { get; init; }

    /// <summary>Gets the new state.</summary>
    public required ConnectionState Current { get; init; }
}
