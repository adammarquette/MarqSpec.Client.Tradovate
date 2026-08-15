using FakeItEasy;
using FluentAssertions;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Api.Rest;
using MarqSpec.Client.Tradovate.Configuration;
using MarqSpec.Client.Tradovate.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarqSpec.Client.Tradovate.Tests.Api;

public sealed class TradovateApiClientTests
{
    [Fact]
    public void ConfiguredHost_ShouldSurfaceRestBaseUrl_WhenClientIsConstructed()
    {
        TradovateApiClient client = CreateClient(A.Fake<ITradovateRestApi>());

        client.ConfiguredHost.Should().Be("https://demo.tradovateapi.com/v1");
    }

    [Fact]
    public async Task PlaceOrderAsync_ShouldThrow_WhenFailureReasonIsNotSuccess()
    {
        ITradovateRestApi rest = A.Fake<ITradovateRestApi>();
        A.CallTo(() => rest.PlaceOrderAsync(A<PlaceOrder>._, A<CancellationToken>._))
            .Returns(new PlaceOrderResult { FailureReason = FailureReason.InvalidContract, FailureText = "no such symbol" });

        TradovateApiClient client = CreateClient(rest);

        Func<Task> act = () => client.PlaceOrderAsync(ValidPlace());

        TradovateApiException ex = (await act.Should().ThrowAsync<TradovateApiException>()).Which;
        ex.FailureReason.Should().Be(FailureReason.InvalidContract);
        ex.Message.Should().Contain("placeOrder");
    }

    [Fact]
    public async Task LiquidatePositionAsync_ShouldThrow_WhenFailureReasonIsTradingLocked()
    {
        ITradovateRestApi rest = A.Fake<ITradovateRestApi>();
        A.CallTo(() => rest.LiquidatePositionAsync(A<LiquidatePosition>._, A<CancellationToken>._))
            .Returns(new CommandResult { FailureReason = FailureReason.TradingLocked });

        TradovateApiClient client = CreateClient(rest);

        Func<Task> act = () => client.LiquidatePositionAsync(new LiquidatePosition
        {
            AccountId = 1,
            ContractId = 2,
            Admin = false,
        });

        (await act.Should().ThrowAsync<TradovateApiException>()).Which.FailureReason.Should().Be(FailureReason.TradingLocked);
    }

    [Fact]
    public async Task PlaceOrderAsync_ShouldReturnResult_WhenFailureReasonIsSuccess()
    {
        ITradovateRestApi rest = A.Fake<ITradovateRestApi>();
        A.CallTo(() => rest.PlaceOrderAsync(A<PlaceOrder>._, A<CancellationToken>._))
            .Returns(new PlaceOrderResult { FailureReason = FailureReason.Success, OrderId = 88 });

        TradovateApiClient client = CreateClient(rest);

        PlaceOrderResult result = await client.PlaceOrderAsync(ValidPlace());

        result.OrderId.Should().Be(88);
        A.CallTo(() => rest.PlaceOrderAsync(A<PlaceOrder>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetPositionsAsync_ShouldKeepSignedNetPos()
    {
        ITradovateRestApi rest = A.Fake<ITradovateRestApi>();
        A.CallTo(() => rest.ListPositionsAsync(A<CancellationToken>._))
            .Returns(
            [
                new Position
                {
                    AccountId = 1,
                    ContractId = 2,
                    Timestamp = DateTimeOffset.UtcNow,
                    TradeDate = new TradeDate { Year = 2026, Month = 8, Day = 15 },
                    NetPos = -4,
                    Bought = 0,
                    BoughtValue = 0m,
                    Sold = 4,
                    SoldValue = 1m,
                    PrevPos = 0,
                },
            ]);

        TradovateApiClient client = CreateClient(rest);

        IReadOnlyList<Position> positions = await client.GetPositionsAsync();

        positions.Should().ContainSingle().Which.NetPos.Should().Be(-4);
    }

    [Fact]
    public async Task GetAccountsAsync_ShouldReturnAccounts()
    {
        ITradovateRestApi rest = A.Fake<ITradovateRestApi>();
        A.CallTo(() => rest.ListAccountsAsync(A<CancellationToken>._))
            .Returns(
            [
                new Account
                {
                    Id = 9,
                    Name = "DEMO",
                    UserId = 1,
                    AccountType = AccountType.Customer,
                    Active = true,
                    ClearingHouseId = 1,
                    RiskCategoryId = 1,
                    AutoLiqProfileId = 1,
                    MarginAccountType = MarginAccountType.Speculator,
                    LegalStatus = LegalStatus.Individual,
                },
            ]);

        TradovateApiClient client = CreateClient(rest);

        IReadOnlyList<Account> accounts = await client.GetAccountsAsync();

        accounts.Should().ContainSingle().Which.Id.Should().Be(9);
    }

    private static PlaceOrder ValidPlace()
    {
        return new PlaceOrder
        {
            AccountSpec = "DEMO",
            Action = OrderAction.Buy,
            Symbol = "ESM24",
            OrderQty = 1,
            OrderType = OrderType.Market,
            IsAutomated = true,
        };
    }

    private static TradovateApiClient CreateClient(ITradovateRestApi rest)
    {
        var options = Options.Create(new TradovateOptions
        {
            Username = "demo-user",
            Password = "demo-pass",
            RestBaseUrl = "https://demo.tradovateapi.com/v1",
            TradingSocketUrl = "wss://demo.tradovateapi.com/v1/websocket",
            MarketDataSocketUrl = "wss://md.tradovateapi.com/v1/websocket",
        });

        return new TradovateApiClient(rest, options, A.Fake<ILogger<TradovateApiClient>>());
    }
}
