using FluentAssertions;
using MarqSpec.Client.Tradovate.Internal;

namespace MarqSpec.Client.Tradovate.Tests.Internal;

public sealed class CommandEndpointsTests
{
    [Theory]
    [InlineData("/v1/order/placeorder")]
    [InlineData("/v1/order/placeoso")]
    [InlineData("/v1/order/placeoco")]
    [InlineData("/v1/order/liquidateposition")]
    [InlineData("/ORDER/PLACEORDER")]
    public void IsNeverRetried_ShouldBeTrue_WhenPathIsACommand(string path)
    {
        CommandEndpoints.IsNeverRetried(path).Should().BeTrue();
        CommandEndpoints.IsNeverRetried(new Uri("https://demo.tradovateapi.com" + path)).Should().BeTrue();
    }

    [Theory]
    [InlineData("/v1/order/cancelorder")]
    [InlineData("/v1/order/modifyorder")]
    [InlineData("/v1/account/list")]
    [InlineData("/v1/position/list")]
    public void IsNeverRetried_ShouldBeFalse_WhenPathIsIdempotent(string path)
    {
        CommandEndpoints.IsNeverRetried(path).Should().BeFalse();
    }
}
