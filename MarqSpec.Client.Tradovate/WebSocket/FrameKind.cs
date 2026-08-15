namespace MarqSpec.Client.Tradovate.WebSocket;

/// <summary>
/// Tradovate application-frame kind.
/// </summary>
internal enum FrameKind
{
    Open,
    Heartbeat,
    Close,
    Payload,
    Unknown,
}
