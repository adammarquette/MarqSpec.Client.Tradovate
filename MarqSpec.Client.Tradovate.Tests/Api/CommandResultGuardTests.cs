using FluentAssertions;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Exceptions;
using MarqSpec.Client.Tradovate.Internal;

namespace MarqSpec.Client.Tradovate.Tests.Api;

public sealed class CommandResultGuardTests
{
    [Theory]
    [InlineData(FailureReason.InvalidContract)]
    [InlineData(FailureReason.TradingLocked)]
    [InlineData(FailureReason.UnknownReason)]
    public void EnsureSuccess_ShouldThrow_WhenFailureReasonIsNotSuccess(FailureReason reason)
    {
        Action act = () => CommandResultGuard.EnsureSuccess(reason, "rejected", orderId: 99, operation: "placeOrder");

        TradovateApiException ex = act.Should().Throw<TradovateApiException>().Which;
        ex.FailureReason.Should().Be(reason);
        ex.Message.Should().Contain("placeOrder");
        ex.Message.Should().Contain(reason.ToString());
        ex.Message.Should().NotContain("token", "tokens must never appear in exception text");
    }

    [Fact]
    public void EnsureSuccess_ShouldNotThrow_WhenFailureReasonIsSuccess()
    {
        Action act = () => CommandResultGuard.EnsureSuccess(FailureReason.Success, null, orderId: 10, operation: "placeOrder");

        act.Should().NotThrow();
    }

    [Fact]
    public void EnsureSuccess_ShouldThrow_WhenFailureReasonIsMissingAndOrderIdIsMissing()
    {
        Action act = () => CommandResultGuard.EnsureSuccess(null, null, orderId: null, operation: "placeOrder", requireOrderId: true);

        act.Should().Throw<TradovateApiException>()
            .WithMessage("*placeOrder*")
            .Which.FailureReason.Should().BeNull();
    }

    [Fact]
    public void EnsureSuccess_ShouldNotThrow_WhenFailureReasonIsMissingAndOrderIdIsPresent()
    {
        Action act = () => CommandResultGuard.EnsureSuccess(null, null, orderId: 7, operation: "placeOrder", requireOrderId: true);

        act.Should().NotThrow();
    }
}
