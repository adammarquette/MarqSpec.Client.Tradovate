using System.Net.WebSockets;
using System.Text;

namespace MarqSpec.Client.Tradovate.WebSocket;

/// <summary>
/// <see cref="ClientWebSocket"/> transport. Application heartbeats are used; auto-ping is not.
/// </summary>
internal sealed class ClientWebSocketTransport : IWebSocketTransport
{
    private ClientWebSocket? _socket;

    public bool IsConnected => _socket?.State == WebSocketState.Open;

    public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
        _socket?.Dispose();
        _socket = new ClientWebSocket();
        await _socket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);
    }

    public async Task SendAsync(string message, CancellationToken cancellationToken)
    {
        if (_socket is null || _socket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("The WebSocket is not connected.");
        }

        byte[] bytes = Encoding.UTF8.GetBytes(message);
        await _socket.SendAsync(bytes, WebSocketMessageType.Text, endOfMessage: true, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string?> ReceiveAsync(CancellationToken cancellationToken)
    {
        if (_socket is null)
        {
            return null;
        }

        using var buffer = new MemoryStream();
        var chunk = new byte[8192];
        WebSocketReceiveResult result;
        do
        {
            result = await _socket.ReceiveAsync(chunk, cancellationToken).ConfigureAwait(false);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                return null;
            }

            buffer.Write(chunk, 0, result.Count);
        }
        while (!result.EndOfMessage);

        return Encoding.UTF8.GetString(buffer.ToArray());
    }

    public async Task CloseAsync(CancellationToken cancellationToken)
    {
        if (_socket is { State: WebSocketState.Open or WebSocketState.CloseReceived })
        {
            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", cancellationToken).ConfigureAwait(false);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_socket is not null)
        {
            try
            {
                await CloseAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (WebSocketException)
            {
            }

            _socket.Dispose();
            _socket = null;
        }
    }
}
