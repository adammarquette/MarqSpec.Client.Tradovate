using FluentAssertions;
using MarqSpec.Client.Tradovate.WebSocket;

namespace MarqSpec.Client.Tradovate.Tests.WebSocket;

public sealed class FrameProtocolTests
{
    [Fact]
    public void FormatRequest_ShouldUseOfficialFourLineLayout()
    {
        string frame = FrameProtocol.FormatRequest("user/syncrequest", 7, queryParams: null, body: """{"users":[1]}""");

        frame.Should().Be("user/syncrequest\n7\n\n{\"users\":[1]}");
    }

    [Fact]
    public void Parse_ShouldIdentifyOpenHeartbeatAndClose()
    {
        FrameProtocol.Parse("o").Kind.Should().Be(FrameKind.Open);
        FrameProtocol.Parse("h").Kind.Should().Be(FrameKind.Heartbeat);
        FrameProtocol.Parse("c").Kind.Should().Be(FrameKind.Close);
    }

    [Fact]
    public void Parse_ShouldCorrelatePayloadsOnRequestId()
    {
        FrameParseResult result = FrameProtocol.Parse("""a[{"i":3,"s":200,"d":{"orderId":9}},{"e":"props","d":{"entityType":"order"}}]""");

        result.Kind.Should().Be(FrameKind.Payload);
        result.Payloads.Should().HaveCount(2);
        result.Payloads[0].RequestId.Should().Be(3);
        result.Payloads[0].Status.Should().Be(200);
        result.Payloads[1].Event.Should().Be("props");
    }

    [Fact]
    public void HeartbeatBody_ShouldBeEmptyArray()
    {
        FrameProtocol.HeartbeatBody.Should().Be("[]");
    }
}
