using System.Text.Json;

namespace MarqSpec.Client.Tradovate.WebSocket;

/// <summary>
/// One object from an <c>a[…]</c> payload frame.
/// </summary>
internal sealed record FramePayload
{
    public int? RequestId { get; init; }

    public int? Status { get; init; }

    public string? Event { get; init; }

    public JsonElement? Data { get; init; }
}
