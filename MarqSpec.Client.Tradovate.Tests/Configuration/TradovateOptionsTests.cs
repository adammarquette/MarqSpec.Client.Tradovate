using FluentAssertions;
using MarqSpec.Client.Tradovate.Configuration;

namespace MarqSpec.Client.Tradovate.Tests.Configuration;

public sealed class TradovateOptionsTests
{
    [Fact]
    public void Validate_ShouldThrow_WhenRestBaseUrlIsMissing()
    {
        TradovateOptions options = ValidOptions();
        options.RestBaseUrl = " ";

        Action act = options.Validate;

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*RestBaseUrl*")
            .Which.Message.Should().NotContain("password", "credentials must never appear in exception text");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenTradingSocketUrlIsMissing()
    {
        TradovateOptions options = ValidOptions();
        options.TradingSocketUrl = string.Empty;

        Action act = options.Validate;

        act.Should().Throw<InvalidOperationException>().WithMessage("*TradingSocketUrl*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenMarketDataSocketUrlIsMissing()
    {
        TradovateOptions options = ValidOptions();
        options.MarketDataSocketUrl = string.Empty;

        Action act = options.Validate;

        act.Should().Throw<InvalidOperationException>().WithMessage("*MarketDataSocketUrl*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenUsernameIsMissing()
    {
        TradovateOptions options = ValidOptions();
        options.Username = string.Empty;

        Action act = options.Validate;

        act.Should().Throw<InvalidOperationException>().WithMessage("*Username*");
    }

    [Fact]
    public void Validate_ShouldThrow_WhenPasswordIsMissing()
    {
        TradovateOptions options = ValidOptions();
        options.Password = "secret-must-not-leak";
        options.Password = string.Empty;

        Action act = options.Validate;

        InvalidOperationException ex = act.Should().Throw<InvalidOperationException>().Which;
        ex.Message.Should().Contain("Password");
        ex.Message.Should().NotContain("secret-must-not-leak");
    }

    [Fact]
    public void RestBaseUrl_ShouldHaveNoDefault_WhenConstructed()
    {
        var options = new TradovateOptions();

        options.RestBaseUrl.Should().BeEmpty("host is required with no default and no live fallback");
        options.TradingSocketUrl.Should().BeEmpty();
        options.MarketDataSocketUrl.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldSucceed_WhenRequiredHostAndCredentialsAreSet()
    {
        TradovateOptions options = ValidOptions();

        Action act = options.Validate;

        act.Should().NotThrow();
    }

    private static TradovateOptions ValidOptions()
    {
        return new TradovateOptions
        {
            Username = "demo-user",
            Password = "demo-pass",
            RestBaseUrl = "https://demo.tradovateapi.com/v1",
            TradingSocketUrl = "wss://demo.tradovateapi.com/v1/websocket",
            MarketDataSocketUrl = "wss://md.tradovateapi.com/v1/websocket",
        };
    }
}
