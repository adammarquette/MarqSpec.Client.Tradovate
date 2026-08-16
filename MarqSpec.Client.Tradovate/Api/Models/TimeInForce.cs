namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate time-in-force.
/// </summary>
public enum TimeInForce
{
    /// <summary>Day.</summary>
    Day = 0,

    /// <summary>Fill-or-kill.</summary>
    FOK,

    /// <summary>Good-til-canceled.</summary>
    GTC,

    /// <summary>Good-til-date.</summary>
    GTD,

    /// <summary>Immediate-or-cancel.</summary>
    IOC,
}
