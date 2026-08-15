namespace MarqSpec.Client.Tradovate.WebSocket;

/// <summary>
/// Thin wrapper over a raw WebSocket so tests can substitute a fake.
/// </summary>
internal interface IWebSocketTransport : IAsyncDisposable
{
    bool IsConnected { get; }

    Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

    Task SendAsync(string message, CancellationToken cancellationToken);

    Task<string?> ReceiveAsync(CancellationToken cancellationToken);

    Task CloseAsync(CancellationToken cancellationToken);
}
