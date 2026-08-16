namespace MarqSpec.Client.Tradovate.WebSocket;

/// <summary>
/// Result of parsing one Tradovate WebSocket text frame.
/// </summary>
internal sealed record FrameParseResult
{
    public required FrameKind Kind { get; init; }

    public IReadOnlyList<FramePayload> Payloads { get; init; } = [];
}
