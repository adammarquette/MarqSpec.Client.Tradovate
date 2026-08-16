using System.Text.Json;
using FluentAssertions;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Serialization;

namespace MarqSpec.Client.Tradovate.Tests.Models;

public sealed class QuoteAbsentVsZeroTests
{
    [Fact]
    public void Deserialize_ShouldLeaveBidAndAskSizeNull_WhenSizesAreAbsent()
    {
        const string json = """{"contract":"ESM24","bidPrice":5000.25,"askPrice":5000.50,"lastPrice":5000.25}""";

        Quote? quote = JsonSerializer.Deserialize(json, TradovateJsonSerializerContext.Default.Quote);

        quote.Should().NotBeNull();
        quote!.BidPrice.Should().Be(5000.25m);
        quote.AskPrice.Should().Be(5000.50m);
        quote.BidSize.Should().BeNull("absent size is not zero");
        quote.AskSize.Should().BeNull("absent size is not zero");
    }

    [Fact]
    public void Deserialize_ShouldKeepZeroSize_WhenSocketSendsZero()
    {
        const string json = """{"contract":"ESM24","bidPrice":5000.25,"bidSize":0,"askPrice":5000.50,"askSize":0}""";

        Quote? quote = JsonSerializer.Deserialize(json, TradovateJsonSerializerContext.Default.Quote);

        quote.Should().NotBeNull();
        quote!.BidSize.Should().Be(0);
        quote.AskSize.Should().Be(0);
    }
}
