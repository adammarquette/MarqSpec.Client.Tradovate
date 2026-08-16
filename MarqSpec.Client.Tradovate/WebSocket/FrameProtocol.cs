using System.Text.Json;

namespace MarqSpec.Client.Tradovate.WebSocket;

/// <summary>
/// Formats and parses Tradovate application frames.
/// </summary>
/// <remarks>
/// Send: <c>{endpoint}\n{requestId}\n{queryParams}\n{body}</c>.
/// Recv: <c>o</c> open, <c>h</c> heartbeat, <c>c</c> close, <c>a[{e,i,s,d},…]</c> payloads.
/// Client heartbeat body is <c>[]</c>.
/// reference: tradovate/example-api-csharp-trading WebSocketClient
/// </remarks>
internal static class FrameProtocol
{
    public const string HeartbeatBody = "[]";

    public static string FormatRequest(string endpoint, int requestId, string? queryParams, string? body)
    {
        return $"{endpoint}\n{requestId}\n{queryParams ?? string.Empty}\n{body ?? string.Empty}";
    }

    public static FrameParseResult Parse(string raw)
    {
        if (raw.Length == 0)
        {
            return new FrameParseResult { Kind = FrameKind.Unknown };
        }

        char prefix = raw[0];
        return prefix switch
        {
            'o' => new FrameParseResult { Kind = FrameKind.Open },
            'h' => new FrameParseResult { Kind = FrameKind.Heartbeat },
            'c' => new FrameParseResult { Kind = FrameKind.Close },
            'a' => ParsePayloads(raw),
            _ => new FrameParseResult { Kind = FrameKind.Unknown },
        };
    }

    private static FrameParseResult ParsePayloads(string raw)
    {
        if (raw.Length < 2)
        {
            return new FrameParseResult { Kind = FrameKind.Payload };
        }

        using JsonDocument document = JsonDocument.Parse(raw[1..]);
        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            return new FrameParseResult { Kind = FrameKind.Payload };
        }

        var payloads = new List<FramePayload>(document.RootElement.GetArrayLength());
        foreach (JsonElement item in document.RootElement.EnumerateArray())
        {
            payloads.Add(new FramePayload
            {
                RequestId = ReadInt(item, "i"),
                Status = ReadInt(item, "s"),
                Event = ReadString(item, "e"),
                Data = item.TryGetProperty("d", out JsonElement data) ? data.Clone() : null,
            });
        }

        return new FrameParseResult { Kind = FrameKind.Payload, Payloads = payloads };
    }

    private static int? ReadInt(JsonElement item, string name)
    {
        if (!item.TryGetProperty(name, out JsonElement value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out int number))
        {
            return number;
        }

        return null;
    }

    private static string? ReadString(JsonElement item, string name)
    {
        if (!item.TryGetProperty(name, out JsonElement value) || value.ValueKind != JsonValueKind.String)
        {
            return null;
        }

        return value.GetString();
    }
}
