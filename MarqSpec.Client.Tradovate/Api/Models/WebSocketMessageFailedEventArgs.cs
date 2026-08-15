namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Raised when a WebSocket subscribe or invoke fails to send.
/// </summary>
public sealed class WebSocketMessageFailedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebSocketMessageFailedEventArgs"/> class.
    /// </summary>
    /// <param name="endpoint">The frame endpoint that failed.</param>
    /// <param name="message">A safe error message (no tokens).</param>
    /// <param name="isTradingSocket"><see langword="true"/> when the trading socket failed; otherwise the market-data socket.</param>
    public WebSocketMessageFailedEventArgs(string endpoint, string message, bool isTradingSocket)
    {
        Endpoint = endpoint;
        Message = message;
        IsTradingSocket = isTradingSocket;
    }

    /// <summary>Gets the frame endpoint that failed.</summary>
    public string Endpoint { get; }

    /// <summary>Gets a safe error message. Never includes tokens.</summary>
    public string Message { get; }

    /// <summary>Gets a value indicating whether the trading socket failed.</summary>
    public bool IsTradingSocket { get; }
}
