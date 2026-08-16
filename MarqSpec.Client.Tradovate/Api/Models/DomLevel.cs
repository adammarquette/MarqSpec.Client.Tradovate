namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// One price level in a Tradovate depth-of-market book.
/// </summary>
public sealed record DomLevel
{
    /// <summary>Gets the price.</summary>
    public required decimal Price { get; init; }

    /// <summary>Gets the size at this price. Null when the socket omitted it.</summary>
    public int? Size { get; init; }
}
