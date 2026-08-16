using System.Text.Json;
using FluentAssertions;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Serialization;

namespace MarqSpec.Client.Tradovate.Tests.Models;

public sealed class ProductUnitsTests
{
    [Fact]
    public void Deserialize_ShouldExposeValuePerPointAndTickSizeSeparately_WhenBothArePresent()
    {
        const string json = """
            {
              "name": "ES",
              "currencyId": 1,
              "productType": "Futures",
              "description": "E-mini S&P 500",
              "exchangeId": 1,
              "contractGroupId": 1,
              "status": "Verified",
              "valuePerPoint": 50,
              "priceFormatType": "Decimal",
              "priceFormat": 2,
              "tickSize": 0.25
            }
            """;

        Product? product = JsonSerializer.Deserialize(json, TradovateJsonSerializerContext.Default.Product);

        product.Should().NotBeNull();
        product!.ValuePerPoint.Should().Be(50m, "Tradovate valuePerPoint is point value, not tick value");
        product.TickSize.Should().Be(0.25m);
        (product.ValuePerPoint / product.TickSize).Should().Be(200m, "do not bake PointValue = TickValue / TickSize into the model");
    }
}
