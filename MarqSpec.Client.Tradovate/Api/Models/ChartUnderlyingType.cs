namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate chart bar underlying type for <c>md/getChart</c>.
/// </summary>
public enum ChartUnderlyingType
{
    /// <summary>Minute bars.</summary>
    MinuteBar = 0,

    /// <summary>Tick bars.</summary>
    TickBar,

    /// <summary>Daily bars.</summary>
    DailyBar,
}
