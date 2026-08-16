using System.Text.Json;

namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// A typed view of a trading-socket <c>props</c> event.
/// </summary>
public sealed record EntityPropsEvent
{
    /// <summary>Gets the entity type (e.g. <c>order</c>, <c>position</c>, <c>fill</c>).</summary>
    public required string EntityType { get; init; }

    /// <summary>Gets the event type (<c>Created</c>, <c>Updated</c>, <c>Deleted</c>), when present.</summary>
    public string? EventType { get; init; }

    /// <summary>Gets the raw entity payload.</summary>
    public required JsonElement Entity { get; init; }
}
