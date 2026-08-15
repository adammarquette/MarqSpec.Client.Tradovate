using System.Text.Json;
using MarqSpec.Client.Tradovate.Api.Models;

namespace MarqSpec.Client.Tradovate.WebSocket;

internal static class MarketDataParser
{
    public static IReadOnlyList<Quote> ReadQuotes(JsonElement data)
    {
        var quotes = new List<Quote>();
        if (TryGetArray(data, "quotes", out JsonElement array))
        {
            foreach (JsonElement item in array.EnumerateArray())
            {
                quotes.Add(ReadQuote(item));
            }
        }
        else if (data.ValueKind == JsonValueKind.Object && data.TryGetProperty("entries", out _))
        {
            quotes.Add(ReadQuote(data));
        }

        return quotes;
    }

    public static IReadOnlyList<DomBook> ReadDom(JsonElement data)
    {
        var books = new List<DomBook>();
        if (TryGetArray(data, "doms", out JsonElement array))
        {
            foreach (JsonElement item in array.EnumerateArray())
            {
                books.Add(ReadBook(item));
            }
        }
        else if (data.ValueKind == JsonValueKind.Object && (data.TryGetProperty("bids", out _) || data.TryGetProperty("asks", out _)))
        {
            books.Add(ReadBook(data));
        }

        return books;
    }

    public static IReadOnlyList<ChartBar> ReadBars(JsonElement data)
    {
        if (TryGetArray(data, "bars", out JsonElement bars))
        {
            return ReadBarArray(bars);
        }

        if (TryGetArray(data, "charts", out JsonElement charts))
        {
            var all = new List<ChartBar>();
            foreach (JsonElement chart in charts.EnumerateArray())
            {
                if (chart.TryGetProperty("bars", out JsonElement nested) && nested.ValueKind == JsonValueKind.Array)
                {
                    all.AddRange(ReadBarArray(nested));
                }
            }

            return all;
        }

        return [];
    }

    public static Quote ReadQuote(JsonElement item)
    {
        decimal? bidPrice = null;
        int? bidSize = null;
        decimal? askPrice = null;
        int? askSize = null;
        decimal? lastPrice = null;
        int? lastSize = null;

        if (item.TryGetProperty("entries", out JsonElement entries))
        {
            ReadEntry(entries, "Bid", out bidPrice, out bidSize);
            ReadEntry(entries, "Offer", out askPrice, out askSize);
            if (askPrice is null)
            {
                ReadEntry(entries, "Ask", out askPrice, out askSize);
            }

            ReadEntry(entries, "Trade", out lastPrice, out lastSize);
        }
        else
        {
            bidPrice = ReadDecimal(item, "bidPrice");
            bidSize = ReadInt(item, "bidSize");
            askPrice = ReadDecimal(item, "askPrice");
            askSize = ReadInt(item, "askSize");
            lastPrice = ReadDecimal(item, "lastPrice");
            lastSize = ReadInt(item, "lastSize");
        }

        return new Quote
        {
            Contract = ReadString(item, "contract") ?? ReadString(item, "symbol"),
            ContractId = ReadInt64(item, "contractId"),
            Timestamp = ReadTimestamp(item),
            BidPrice = bidPrice,
            BidSize = bidSize,
            AskPrice = askPrice,
            AskSize = askSize,
            LastPrice = lastPrice,
            LastSize = lastSize,
        };
    }

    private static DomBook ReadBook(JsonElement item)
    {
        return new DomBook
        {
            Contract = ReadString(item, "contract") ?? ReadString(item, "symbol"),
            ContractId = ReadInt64(item, "contractId"),
            Timestamp = ReadTimestamp(item),
            Bids = ReadLevels(item, "bids"),
            Asks = ReadLevels(item, "asks"),
        };
    }

    private static IReadOnlyList<DomLevel> ReadLevels(JsonElement item, string name)
    {
        if (!item.TryGetProperty(name, out JsonElement array) || array.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var levels = new List<DomLevel>();
        foreach (JsonElement level in array.EnumerateArray())
        {
            decimal? price = ReadDecimal(level, "price");
            if (price is null)
            {
                continue;
            }

            levels.Add(new DomLevel { Price = price.Value, Size = ReadInt(level, "size") });
        }

        return levels;
    }

    private static IReadOnlyList<ChartBar> ReadBarArray(JsonElement bars)
    {
        var list = new List<ChartBar>();
        foreach (JsonElement bar in bars.EnumerateArray())
        {
            DateTimeOffset? timestamp = ReadTimestamp(bar);
            decimal? open = ReadDecimal(bar, "open");
            decimal? high = ReadDecimal(bar, "high");
            decimal? low = ReadDecimal(bar, "low");
            decimal? close = ReadDecimal(bar, "close");
            if (timestamp is null || open is null || high is null || low is null || close is null)
            {
                continue;
            }

            list.Add(new ChartBar
            {
                Timestamp = timestamp.Value,
                Open = open.Value,
                High = high.Value,
                Low = low.Value,
                Close = close.Value,
                Volume = ReadDecimal(bar, "volume"),
                UpVolume = ReadDecimal(bar, "upVolume"),
                DownVolume = ReadDecimal(bar, "downVolume"),
            });
        }

        return list;
    }

    private static void ReadEntry(JsonElement entries, string name, out decimal? price, out int? size)
    {
        price = null;
        size = null;
        if (!entries.TryGetProperty(name, out JsonElement entry))
        {
            return;
        }

        price = ReadDecimal(entry, "price");
        size = ReadInt(entry, "size");
    }

    private static bool TryGetArray(JsonElement data, string name, out JsonElement array)
    {
        if (data.ValueKind == JsonValueKind.Object && data.TryGetProperty(name, out array) && array.ValueKind == JsonValueKind.Array)
        {
            return true;
        }

        array = default;
        return false;
    }

    private static string? ReadString(JsonElement item, string name)
    {
        return item.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }

    private static int? ReadInt(JsonElement item, string name)
    {
        if (!item.TryGetProperty(name, out JsonElement value) || value.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        return value.TryGetInt32(out int number) ? number : null;
    }

    private static long? ReadInt64(JsonElement item, string name)
    {
        if (!item.TryGetProperty(name, out JsonElement value) || value.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        return value.TryGetInt64(out long number) ? number : null;
    }

    private static decimal? ReadDecimal(JsonElement item, string name)
    {
        if (!item.TryGetProperty(name, out JsonElement value) || value.ValueKind != JsonValueKind.Number)
        {
            return null;
        }

        return value.TryGetDecimal(out decimal number) ? number : null;
    }

    private static DateTimeOffset? ReadTimestamp(JsonElement item)
    {
        if (!item.TryGetProperty("timestamp", out JsonElement value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.String && DateTimeOffset.TryParse(value.GetString(), out DateTimeOffset parsed))
        {
            return parsed;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out long unix))
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unix);
        }

        return null;
    }
}
