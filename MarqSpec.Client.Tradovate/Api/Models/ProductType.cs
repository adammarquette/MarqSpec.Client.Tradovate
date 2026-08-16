namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate product type.
/// </summary>
public enum ProductType
{
    /// <summary>Futures.</summary>
    Futures = 0,

    /// <summary>Common stock.</summary>
    CommonStock,

    /// <summary>Continuous contract.</summary>
    Continuous,

    /// <summary>Cryptocurrency.</summary>
    Cryptocurrency,

    /// <summary>Market internals.</summary>
    MarketInternals,

    /// <summary>Options.</summary>
    Options,

    /// <summary>Spread.</summary>
    Spread,
}
