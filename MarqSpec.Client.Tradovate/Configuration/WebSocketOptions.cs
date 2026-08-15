namespace MarqSpec.Client.Tradovate.Configuration;

/// <summary>
/// WebSocket heartbeat and stall-detection options.
/// </summary>
/// <remarks>
/// Official sample cadence: client sends <c>[]</c> every 2.5s; ~10s of server silence means reconnect.
/// Websocket-level ping is disabled — Tradovate uses application frames.
/// </remarks>
public sealed class WebSocketOptions
{
    /// <summary>
    /// Gets or sets the interval between client heartbeat frames. Default is 2.5 seconds.
    /// </summary>
    public TimeSpan HeartbeatInterval { get; set; } = TimeSpan.FromMilliseconds(2500);

    /// <summary>
    /// Gets or sets how long the server may stay silent before the socket is treated as dead. Default is 10 seconds.
    /// </summary>
    public TimeSpan ServerSilenceTimeout { get; set; } = TimeSpan.FromSeconds(10);
}
