using System.Text.Json.Serialization;
using MarqSpec.Client.Tradovate.Api.Models;

namespace MarqSpec.Client.Tradovate.Serialization;

/// <summary>
/// Source-generated JSON contract for Tradovate wire types.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    UseStringEnumConverter = true,
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(Account))]
[JsonSerializable(typeof(Account[]))]
[JsonSerializable(typeof(AccessTokenRequest))]
[JsonSerializable(typeof(AccessTokenResponse))]
[JsonSerializable(typeof(AuthMe))]
[JsonSerializable(typeof(CancelOrder))]
[JsonSerializable(typeof(CashBalance))]
[JsonSerializable(typeof(CashBalance[]))]
[JsonSerializable(typeof(ChartBar))]
[JsonSerializable(typeof(ChartBar[]))]
[JsonSerializable(typeof(ChartRequest))]
[JsonSerializable(typeof(CommandResult))]
[JsonSerializable(typeof(Contract))]
[JsonSerializable(typeof(Contract[]))]
[JsonSerializable(typeof(ContractMaturity))]
[JsonSerializable(typeof(DomBook))]
[JsonSerializable(typeof(DomLevel))]
[JsonSerializable(typeof(EntityPropsEvent))]
[JsonSerializable(typeof(Fill))]
[JsonSerializable(typeof(Fill[]))]
[JsonSerializable(typeof(LiquidatePosition))]
[JsonSerializable(typeof(ModifyOrder))]
[JsonSerializable(typeof(Order))]
[JsonSerializable(typeof(Order[]))]
[JsonSerializable(typeof(PlaceOco))]
[JsonSerializable(typeof(PlaceOcoResult))]
[JsonSerializable(typeof(PlaceOrder))]
[JsonSerializable(typeof(PlaceOrderResult))]
[JsonSerializable(typeof(PlaceOso))]
[JsonSerializable(typeof(PlaceOsoResult))]
[JsonSerializable(typeof(Position))]
[JsonSerializable(typeof(Position[]))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(Quote))]
[JsonSerializable(typeof(RestrainedOrderVersion))]
[JsonSerializable(typeof(SyncRequest))]
[JsonSerializable(typeof(SyncResult))]
[JsonSerializable(typeof(TradeDate))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(User[]))]
internal sealed partial class TradovateJsonSerializerContext : JsonSerializerContext
{
}
