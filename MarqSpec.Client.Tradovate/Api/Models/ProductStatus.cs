namespace MarqSpec.Client.Tradovate.Api.Models;

/// <summary>
/// Tradovate product status.
/// </summary>
public enum ProductStatus
{
    /// <summary>Verified.</summary>
    Verified = 0,

    /// <summary>Inactive.</summary>
    Inactive,

    /// <summary>Locked.</summary>
    Locked,

    /// <summary>Ready for contracts.</summary>
    ReadyForContracts,

    /// <summary>Ready to trade.</summary>
    ReadyToTrade,
}
