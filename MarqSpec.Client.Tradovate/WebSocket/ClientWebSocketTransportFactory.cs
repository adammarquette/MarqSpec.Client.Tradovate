namespace MarqSpec.Client.Tradovate.WebSocket;

internal sealed class ClientWebSocketTransportFactory : IWebSocketTransportFactory
{
    public IWebSocketTransport Create()
    {
        return new ClientWebSocketTransport();
    }
}
