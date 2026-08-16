namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Connection state of one Tradovate WebSocket.
/// </summary>
public enum ConnectionState
{
    /// <summary>Not connected.</summary>
    Disconnected = 0,

    /// <summary>Connecting or authorizing.</summary>
    Connecting,

    /// <summary>Connected and authorized.</summary>
    Connected,

    /// <summary>Reconnecting after a stall or close.</summary>
    Reconnecting,
}
