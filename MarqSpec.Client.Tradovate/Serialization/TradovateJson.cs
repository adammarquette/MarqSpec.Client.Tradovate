using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarqSpec.Client.Tradovate.Serialization;

/// <summary>
/// Shared <see cref="JsonSerializerOptions"/> for Refit and the frame protocol.
/// </summary>
internal static class TradovateJson
{
    public static JsonSerializerOptions Options { get; } = CreateOptions();

    public static JsonSerializerOptions LooseOptions { get; } = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter() },
    };

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = TradovateJsonSerializerContext.Default,
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
