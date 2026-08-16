using System.Threading.Channels;
using MarqSpec.Client.Tradovate.WebSocket;

namespace MarqSpec.Client.Tradovate.Tests.WebSocket;

internal sealed class FakeWebSocketTransport : IWebSocketTransport
{
    private readonly Channel<string> _incoming = Channel.CreateUnbounded<string>();
    private readonly List<string> _sent = [];

    public bool IsConnected { get; private set; }

    public Uri? ConnectedUri { get; private set; }

    public Exception? ConnectFault { get; set; }

    public Func<string, Exception?>? SendFault { get; set; }

    public IReadOnlyList<string> Sent => _sent;

    public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
        if (ConnectFault is not null)
        {
            throw ConnectFault;
        }

        ConnectedUri = uri;
        IsConnected = true;
        return Task.CompletedTask;
    }

    public Task SendAsync(string message, CancellationToken cancellationToken)
    {
        if (SendFault?.Invoke(message) is { } fault)
        {
            throw fault;
        }

        _sent.Add(message);
        return Task.CompletedTask;
    }

    public async Task<string?> ReceiveAsync(CancellationToken cancellationToken)
    {
        return await _incoming.Reader.ReadAsync(cancellationToken);
    }

    public Task CloseAsync(CancellationToken cancellationToken)
    {
        IsConnected = false;
        return Task.CompletedTask;
    }

    public void Enqueue(string frame)
    {
        _incoming.Writer.TryWrite(frame);
    }

    public ValueTask DisposeAsync()
    {
        IsConnected = false;
        _incoming.Writer.TryComplete();
        return ValueTask.CompletedTask;
    }
}

internal sealed class FakeWebSocketTransportFactory : IWebSocketTransportFactory
{
    public FakeWebSocketTransportFactory(params FakeWebSocketTransport[] transports)
    {
        Transports = [.. transports];
    }

    public List<FakeWebSocketTransport> Transports { get; }

    public int Created { get; private set; }

    public IWebSocketTransport Create()
    {
        FakeWebSocketTransport transport = Transports[Math.Min(Created, Transports.Count - 1)];
        Created++;
        return transport;
    }
}
