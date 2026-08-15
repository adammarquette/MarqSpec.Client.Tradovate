namespace MarqSpec.Client.Tradovate.WebSocket;

internal interface IWebSocketTransportFactory
{
    IWebSocketTransport Create();
}
