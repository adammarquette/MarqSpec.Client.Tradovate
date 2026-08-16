using System.Text.Json;
using MarqSpec.Client.Tradovate.Api.Models;
using MarqSpec.Client.Tradovate.Serialization;

namespace MarqSpec.Client.Tradovate.WebSocket;

internal static class EntitySnapshotParser
{
    public static SyncResult ReadSync(JsonElement data)
    {
        return new SyncResult
        {
            Accounts = ReadList<Account>(data, "accounts"),
            Users = ReadList<User>(data, "users"),
            CashBalances = ReadList<CashBalance>(data, "cashBalances"),
            Contracts = ReadList<Contract>(data, "contracts"),
            Products = ReadList<Product>(data, "products"),
            Positions = ReadList<Position>(data, "positions"),
            Orders = ReadList<Order>(data, "orders"),
            Fills = ReadList<Fill>(data, "fills"),
        };
    }

    public static EntityPropsEvent? ReadProps(JsonElement data)
    {
        string? entityType = ReadString(data, "entityType");
        if (string.IsNullOrWhiteSpace(entityType))
        {
            return null;
        }

        if (!data.TryGetProperty("entity", out JsonElement entity))
        {
            return null;
        }

        return new EntityPropsEvent
        {
            EntityType = entityType,
            EventType = ReadString(data, "eventType"),
            Entity = entity.Clone(),
        };
    }

    public static T? Deserialize<T>(JsonElement element)
        where T : class
    {
        return JsonSerializer.Deserialize(element.GetRawText(), typeof(T), TradovateJson.Options) as T;
    }

    private static IReadOnlyList<T> ReadList<T>(JsonElement data, string name)
    {
        if (!data.TryGetProperty(name, out JsonElement array) || array.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        T[]? items = JsonSerializer.Deserialize<T[]>(array.GetRawText(), TradovateJson.Options);
        return items ?? [];
    }

    private static string? ReadString(JsonElement data, string name)
    {
        return data.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }
}
