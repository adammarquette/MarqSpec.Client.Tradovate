using FluentAssertions;
using MarqSpec.Client.Tradovate.Api.Models;

namespace MarqSpec.Client.Tradovate.Tests.Models;

public sealed class PositionSignTests
{
    [Fact]
    public void NetPos_ShouldPreserveNegativeSign_WhenPositionIsShort()
    {
        var position = new Position
        {
            AccountId = 1,
            ContractId = 2,
            Timestamp = DateTimeOffset.UtcNow,
            TradeDate = new TradeDate { Year = 2026, Month = 8, Day = 15 },
            NetPos = -3,
            Bought = 0,
            BoughtValue = 0m,
            Sold = 3,
            SoldValue = 15000m,
            PrevPos = 0,
        };

        position.NetPos.Should().Be(-3, "Tradovate net qty is signed; dropping the sign would invert a short");
    }

    [Fact]
    public void NetPos_ShouldPreservePositiveSign_WhenPositionIsLong()
    {
        var position = new Position
        {
            AccountId = 1,
            ContractId = 2,
            Timestamp = DateTimeOffset.UtcNow,
            TradeDate = new TradeDate { Year = 2026, Month = 8, Day = 15 },
            NetPos = 2,
            Bought = 2,
            BoughtValue = 10000m,
            Sold = 0,
            SoldValue = 0m,
            PrevPos = -1,
        };

        position.NetPos.Should().Be(2);
        position.PrevPos.Should().Be(-1, "previous session qty is also signed");
    }
}
